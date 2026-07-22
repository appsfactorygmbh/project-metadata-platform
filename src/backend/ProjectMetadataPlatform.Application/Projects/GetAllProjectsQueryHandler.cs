using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <inheritdoc />
public class GetAllProjectsQueryHandler
    : IRequestHandler<
        GetAllProjectsQuery,
        (IEnumerable<Project>, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly IProjectsRepository _projectRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllProjectsQueryHandler" />.
    /// </summary>
    public GetAllProjectsQueryHandler(
        IProjectsRepository projectsRepository,
        IAuthorizationService authorizationService
    )
    {
        _projectRepository = projectsRepository;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Project>, IEnumerable<AuthorizationConstants.Actions>)> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        var projects = await _projectRepository.GetProjectsAsync(request);
        var queriedProjects = await _authorizationService.TryGetPlanResourceQuery(projects);
        var permissions = await _authorizationService.GetPermissions<Project>(            actions: [AuthorizationConstants.Actions.CREATE]);
        if (queriedProjects == null)
        {
            List<Project> filteredProjects = [];
            foreach (var project in projects)
            {
                if (
                    await _authorizationService.CheckAccess(
                        project,
                        AuthorizationConstants.Actions.GET
                    )
                )
                {
                    filteredProjects.Add(project);
                }
            }
            return (
                filteredProjects.OrderBy(project => project.ProjectName.ToLowerInvariant()),
                permissions
            );
        }
        return (
            (await queriedProjects.ToListAsync(cancellationToken: cancellationToken))
                .OrderBy(project => project.ClientName)
                .ThenBy(project => project.ProjectName),
            permissions
        );
    }
}

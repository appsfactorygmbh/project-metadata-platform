using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <inheritdoc />
public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, Project>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetProjectQueryHandler" />.
    /// </summary>
    public GetProjectQueryHandler(
        IProjectsRepository projectsRepository,
        IAuthorizationService authorizationService
    )
    {
        _projectsRepository = projectsRepository;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<Project> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetProjectAsync(request.Id);
        if (
            !(
                await _authorizationService.CheckAccess(
                    project,
                    [AuthorizationConstants.Actions.GET]
                )
            )[AuthorizationConstants.Actions.GET]
        )
        {
            throw new UnauthorizedException();
        }
        return project;
    }
}

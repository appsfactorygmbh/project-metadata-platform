using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <inheritdoc />
public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, IEnumerable<Project>>
{
    private readonly IProjectsRepository _projectRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllProjectsQueryHandler" />.
    /// </summary>
    public GetAllProjectsQueryHandler(IProjectsRepository projectsRepository)
    {
        _projectRepository = projectsRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Project>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        var projects = await _projectRepository.GetProjectsAsync(request);
        return projects
            .OrderBy(project => project.ClientName)
            .ThenBy(project => project.ProjectName);
    }
}

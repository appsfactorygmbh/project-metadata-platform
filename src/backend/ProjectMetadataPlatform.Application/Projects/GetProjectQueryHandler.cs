using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <inheritdoc />
public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, Project>
{
    private readonly IProjectsRepository _projectsRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetProjectQueryHandler" />.
    /// </summary>
    public GetProjectQueryHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    /// <inheritdoc />
    public Task<Project> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        return _projectsRepository.GetProjectAsync(request.Id);
    }
}

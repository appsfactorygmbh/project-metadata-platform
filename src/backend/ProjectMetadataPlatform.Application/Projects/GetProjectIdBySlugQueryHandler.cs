using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handler for the <see cref="GetProjectIdBySlugQuery"/>.
/// </summary>
public class GetProjectIdBySlugQueryHandler : IRequestHandler<GetProjectIdBySlugQuery, int>
{
    private readonly IProjectsRepository _projectsRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetProjectIdBySlugQueryHandler"/>.
    /// </summary>
    public GetProjectIdBySlugQueryHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    /// <summary>
    /// Handles the <see cref="GetProjectIdBySlugQuery"/>.
    /// </summary>
    /// <param name="request">request containing the Slug of a project</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Either a projectId or null.</returns>
    public async Task<int> Handle(
        GetProjectIdBySlugQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _projectsRepository.GetProjectIdBySlugAsync(request.Slug);
    }
}

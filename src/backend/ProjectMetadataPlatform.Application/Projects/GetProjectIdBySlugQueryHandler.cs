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
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetProjectIdBySlugQueryHandler"/>.
    /// </summary>
    public GetProjectIdBySlugQueryHandler(
        IProjectsRepository projectsRepository,
        IAuthorizationService authorizationService
    )
    {
        _projectsRepository = projectsRepository;
        _authorizationService = authorizationService;
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
        //Internal Command
        await _authorizationService.BypassAuthorization();

        return await _projectsRepository.GetProjectIdBySlugAsync(request.Slug);
    }
}

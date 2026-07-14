using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handler for the <see cref="GetAllPluginsForProjectIdQuery" />
/// </summary>
public class GetAllPluginsForProjectIdQueryHandler
    : IRequestHandler<GetAllPluginsForProjectIdQuery, List<ProjectPlugins>>
{
    private readonly IPluginRepository _pluginRepository;

    private readonly IProjectsRepository _projectsRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of<see cref="GetAllPluginsForProjectIdQueryHandler" />.
    /// </summary>
    /// <param name="pluginRepository"></param>
    /// <param name="projectsRepository"></param>
    /// <param name="authorizationService"></param>
    public GetAllPluginsForProjectIdQueryHandler(
        IPluginRepository pluginRepository,
        IProjectsRepository projectsRepository,
        IAuthorizationService authorizationService
    )
    {
        _pluginRepository = pluginRepository;
        _authorizationService = authorizationService;
        _projectsRepository = projectsRepository;
    }

    /// <summary>
    /// Handles the request to get all plugins for a given project id.
    /// </summary>
    /// <param name="request">the request that needs to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the response of the request</returns>
    public async Task<List<ProjectPlugins>> Handle(
        GetAllPluginsForProjectIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var plugins = await _pluginRepository.GetAllPluginsForProjectIdAsync(request.Id);
        if (
            !await _authorizationService.CheckAccess(
                await _projectsRepository.GetProjectAsync(request.Id),
                AuthorizationConstants.Actions.GET
            )
        )
        {
            throw new UnauthorizedException();
        }
        return plugins;
    }
}

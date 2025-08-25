using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handler for the <see cref="GetAllPluginsForProjectIdQuery" />
/// </summary>
public class GetAllPluginsForProjectIdQueryHandler
    : IRequestHandler<GetAllPluginsForProjectIdQuery, List<ProjectPlugins>>
{
    private readonly IPluginRepository _pluginRepository;

    /// <summary>
    /// Creates a new instance of<see cref="GetAllPluginsForProjectIdQueryHandler" />.
    /// </summary>
    /// <param name="pluginRepository"></param>
    public GetAllPluginsForProjectIdQueryHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    /// <summary>
    /// Handles the request to get all plugins for a given project id.
    /// </summary>
    /// <param name="request">the request that needs to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the response of the request</returns>
    public Task<List<ProjectPlugins>> Handle(
        GetAllPluginsForProjectIdQuery request,
        CancellationToken cancellationToken
    )
    {
        return _pluginRepository.GetAllPluginsForProjectIdAsync(request.Id);
    }
}

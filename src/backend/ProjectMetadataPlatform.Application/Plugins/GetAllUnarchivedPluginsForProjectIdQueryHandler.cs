using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handler for the <see cref="GetAllUnarchivedPluginsForProjectIdQuery" />
/// </summary>
public class GetAllUnarchivedPluginsForProjectIdQueryHandler
    : IRequestHandler<GetAllUnarchivedPluginsForProjectIdQuery, List<ProjectPlugins>>
{
    private readonly IPluginRepository _pluginRepository;

    /// <summary>
    /// Creates a new instance of<see cref="GetAllUnarchivedPluginsForProjectIdQueryHandler" />.
    /// </summary>
    /// <param name="pluginRepository"></param>
    public GetAllUnarchivedPluginsForProjectIdQueryHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    /// <summary>
    /// Handles the request to get all unarchived plugins for a given project id.
    /// </summary>
    /// <param name="request">the request that needs to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the response of the request</returns>
    public async Task<List<ProjectPlugins>> Handle(
        GetAllUnarchivedPluginsForProjectIdQuery request,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _pluginRepository.GetAllUnarchivedPluginsForProjectIdAsync(request.Id);
    }
}

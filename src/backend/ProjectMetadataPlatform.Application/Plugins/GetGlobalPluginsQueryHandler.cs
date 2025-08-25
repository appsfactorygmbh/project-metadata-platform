using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

///  <inheritdoc />
public class GetGlobalPluginsQueryHandler
    : IRequestHandler<GetGlobalPluginsQuery, IEnumerable<Plugin>>
{
    private readonly IPluginRepository _pluginRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetGlobalPluginsQueryHandler"/>.
    /// </summary>
    public GetGlobalPluginsQueryHandler(IPluginRepository pluginRepository)
    {
        _pluginRepository = pluginRepository;
    }

    /// <inheritdoc />
    public Task<IEnumerable<Plugin>> Handle(
        GetGlobalPluginsQuery request,
        CancellationToken cancellationToken
    )
    {
        return _pluginRepository.GetGlobalPluginsAsync();
    }
}

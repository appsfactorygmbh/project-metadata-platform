using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Query to get all global plugins.
/// </summary>
public record GetGlobalPluginsQuery : IRequest<IEnumerable<Plugin>>;

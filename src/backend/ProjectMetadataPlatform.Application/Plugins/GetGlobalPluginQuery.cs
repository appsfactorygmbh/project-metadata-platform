using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Query to get a global Plugin by its id.
/// </summary>
/// <param name="Id">Id of the Plugin.</param>
public record GetGlobalPluginQuery(int Id)
    : IRequest<(Plugin, IEnumerable<AuthorizationConstants.Actions>)>;

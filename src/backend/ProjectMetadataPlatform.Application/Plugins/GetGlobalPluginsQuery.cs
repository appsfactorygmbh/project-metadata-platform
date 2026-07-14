using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Query to get all global plugins.
/// </summary>
public record GetGlobalPluginsQuery
    : IRequest<(IEnumerable<Plugin>, IEnumerable<AuthorizationConstants.Actions>)>;

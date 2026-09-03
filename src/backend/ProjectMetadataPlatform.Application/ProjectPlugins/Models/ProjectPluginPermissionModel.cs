using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.ProjectPlugins.Models;

/// <summary>
/// DTO for a project plugin, its permissions and the permission to either get existing billing information or create new one.
/// </summary>
/// <param name="Plugin">The Project Plugin</param>
/// <param name="PluginPermissions">Permissions on the project plugin.</param>
/// <param name="BillingPermissions">Permission on billing information on the plugin.</param>
public record ProjectPluginPermissionModel(
    ProjectPlugin Plugin,
    IEnumerable<AuthorizationConstants.Actions> PluginPermissions,
    IEnumerable<AuthorizationConstants.Actions> BillingPermissions
);

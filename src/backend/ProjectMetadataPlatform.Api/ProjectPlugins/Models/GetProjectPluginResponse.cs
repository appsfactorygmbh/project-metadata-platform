using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.ProjectPlugins.Models;

/// <summary>
/// Response model representing a Plugin of a project.
/// </summary>
/// <param name="ProjectId">Id of the Project of the plugin.</param>
/// <param name="Id">Id of the plugin</param>
/// <param name="PluginName">The name of the plugin.</param>
/// <param name="Url">The URL of this plugin instance in the project.</param>
/// <param name="DisplayName">The name of this plugin instance in the project.</param>
/// <param name="PluginId">The global id of the plugin instance in the project.</param>
/// <param name="PluginPermissions"> Permissions for the plugin.</param>
/// <param name="BillingPermissions">Permissions for the billing information of the plugin.</param>
public record GetProjectPluginResponse(
    int ProjectId,
    int Id,
    string PluginName,
    string Url,
    string DisplayName,
    int PluginId,
    List<AuthorizationConstants.Actions>? PluginPermissions = null,
    List<AuthorizationConstants.Actions>? BillingPermissions = null
);

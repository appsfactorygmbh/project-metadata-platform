namespace ProjectMetadataPlatform.Api.PluginBilling.Models;

/// <summary>
/// Response to adding billing to a project plugin.
/// </summary>
/// <param name="ProjectId">Id of the project.</param>
/// <param name="PluginId">Id of the project plugin.</param>
public record AddPluginBillingResponse(int ProjectId, int PluginId);

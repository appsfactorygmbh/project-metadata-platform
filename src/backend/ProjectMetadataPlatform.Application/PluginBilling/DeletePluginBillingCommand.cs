using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.PluginBilling;

/// <summary>
/// Request for deleting billing information from a plugin.
/// </summary>
/// <param name="ProjectId">Project Id of the billing information. </param>
/// <param name="PluginId">Plugin Id of the billing information.</param>
public record DeletePluginBillingCommand(int ProjectId, int PluginId) : IRequest;

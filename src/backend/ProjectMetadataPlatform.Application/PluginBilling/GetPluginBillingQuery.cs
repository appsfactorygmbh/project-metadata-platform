using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.PluginBilling;

/// <summary>
/// Request to return billing information for a plugin.
/// </summary>
/// <param name="ProjectId">Project Id of the billing information. </param>
/// <param name="PluginId">Plugin Id of the billing information.</param>
public record GetPluginBillingQuery(int ProjectId, int PluginId)
    : IRequest<(Domain.Billing.PluginBilling, IEnumerable<AuthorizationConstants.Actions>)>;

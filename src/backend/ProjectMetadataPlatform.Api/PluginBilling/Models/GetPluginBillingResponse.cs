using System;
using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Api.PluginBilling.Models;

/// <summary>
/// Response for returning billing information for a plugin.
/// </summary>
/// <param name="ProjectId">Id of the project the plugin belongs to.</param>
/// <param name="PluginId">Id of the plugin the billing information belongs to.</param>
/// <param name="BillingId">Id of the global billing object.</param>
/// <param name="DisplayName">Display name.</param>
/// <param name="Currency">Currency Format.</param>
/// <param name="BudgetLimit">Budget Limit.</param>
/// <param name="HostingFee">Hosting Fee.</param>
/// <param name="TargetMargin">Target Margin</param>
/// <param name="TimeFrame">Billing Time frame.</param>
/// <param name="Date"> Billing Date.</param>
/// <param name="Notes"> Notes on the billing information.</param>
/// <param name="Permissions">Permissions on the billing information.</param>
public record GetPluginBillingResponse(
    int ProjectId,
    int PluginId,
    int BillingId,
    string? DisplayName,
    string Currency,
    decimal BudgetLimit,
    decimal HostingFee,
    int TargetMargin,
    TimeFrame TimeFrame,
    DateTimeOffset? Date,
    string? Notes,
    List<AuthorizationConstants.Actions>? Permissions = null
);

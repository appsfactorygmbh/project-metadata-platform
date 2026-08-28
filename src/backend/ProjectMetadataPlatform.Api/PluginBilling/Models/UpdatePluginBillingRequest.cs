using System;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Api.PluginBilling.Models;

/// <summary>
/// Request for updating billing information of a project plugin.
/// </summary>
/// <param name="DisplayName">Optional: Display name of the billing information.</param>
/// <param name="Currency">Currency format</param>
/// <param name="BudgetLimit">Budget Limit</param>
/// <param name="HostingFee">Hosting Fee</param>
/// <param name="TargetMargin">Target Marhin</param>
/// <param name="TimeFrame">Billing TimeFrame</param>
/// <param name="Date">Billing Date. Required if TimeFrame Date was choosen.</param>
/// <param name="Notes">Optional Notes for the billing information.</param>
public record UpdatePluginBillingRequest(
    string? DisplayName,
    string Currency,
    decimal BudgetLimit,
    decimal HostingFee,
    int TargetMargin,
    TimeFrame TimeFrame,
    DateTimeOffset? Date,
    string? Notes
);

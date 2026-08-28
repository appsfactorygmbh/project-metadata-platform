using System;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Api.PluginBilling.Models;

/// <summary>
/// Request for adding billing information to a plugin.
/// </summary>
/// <param name="BillingId">Id of the global billing object.</param>
/// <param name="DisplayName">Optional Displayname.</param>
/// <param name="Currency">Currency Format.</param>
/// <param name="BudgetLimit">Budget Limit.</param>
/// <param name="HostingFee">Hosting Fee.</param>
/// <param name="TargetMargin">Target Margin.</param>
/// <param name="TimeFrame">Billing Time frame.</param>
/// <param name="Date">Billing date required for Time Frame "Date"</param>
/// <param name="Notes">Optional Notes</param>
public record AddPluginBillingRequest(
    int BillingId,
    string? DisplayName,
    string Currency,
    decimal BudgetLimit,
    decimal HostingFee,
    int TargetMargin,
    TimeFrame TimeFrame,
    DateTimeOffset? Date,
    string? Notes
);

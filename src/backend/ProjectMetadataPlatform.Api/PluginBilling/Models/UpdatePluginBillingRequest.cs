using System;
using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Api.PluginBilling.Models;

/// <summary>
/// Request for updating billing information of a project plugin.
/// </summary>
/// <param name="ContractIds">List of Contract Ids</param>
/// <param name="Currency">Currency format</param>
/// <param name="BudgetLimit">Budget Limit</param>
/// <param name="HostingFee">Hosting Fee</param>
/// <param name="TargetMargin">Target Marhin</param>
/// <param name="TimeFrame">Billing TimeFrame</param>
/// <param name="Date">Billing Date. Required if TimeFrame Date was choosen.</param>
/// <param name="Notes">Optional Notes for the billing information.</param>
public record UpdatePluginBillingRequest(
    List<string> ContractIds,
    Currencies Currency,
    decimal BudgetLimit,
    decimal HostingFee,
    int TargetMargin,
    TimeFrame TimeFrame,
    DateTimeOffset? Date,
    string? Notes
);

using System;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Application.PluginBilling;

/// <summary>
/// Command for adding billing information to a plugin.
/// </summary>
/// <param name="ProjectId">Id of project of the plugin</param>
/// <param name="PluginId">Id of the project plugin.</param>
/// <param name="BillingId">Id of the global billing information.</param>
/// <param name="DisplayName">Optional Display name.</param>
/// <param name="Currency">Currency format.</param>
/// <param name="BudgetLimit">Budget limit.</param>
/// <param name="HostingFee">Hosting Fee.</param>
/// <param name="TargetMargin">Target margin percentage.</param>
/// <param name="TimeFrame">Time frame for billing.</param>
/// <param name="Date">Date if timeframe is of type "date".</param>
/// <param name="Notes">Optional notes on the billing information.</param>
public record AddPluginBillingCommand(
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
    string? Notes
) : IRequest<(int, int)>;

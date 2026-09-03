using System;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Application.PluginBilling;

/// <summary>
/// Request to update plugin billing information.
/// </summary>
/// <param name="ProjectId">Id of the Project.</param>
/// <param name="PluginId">Id of the Project Plugin.</param>
/// <param name="DisplayName">Displayname for the Billing Object.</param>
/// <param name="Currency">Currency format for the billing information.</param>
/// <param name="BudgetLimit">Budget Limit.</param>
/// <param name="HostingFee">Hosting Fee.</param>
/// <param name="TargetMargin">Target Margin percentage.</param>
/// <param name="TimeFrame">Billing Time frame.</param>
/// <param name="Date">Date if billing time frame is of type "date"</param>
/// <param name="Notes">Optional Notes on the billing information.</param>
public record UpdatePluginBillingCommand(
    int ProjectId,
    int PluginId,
    string? DisplayName,
    string Currency,
    decimal BudgetLimit,
    decimal HostingFee,
    int TargetMargin,
    TimeFrame TimeFrame,
    DateTimeOffset? Date,
    string? Notes
) : IRequest<Domain.Billing.PluginBilling>;

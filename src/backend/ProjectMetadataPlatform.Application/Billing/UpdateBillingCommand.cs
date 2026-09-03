using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Application.Billing;

/// <summary>
/// Request to update global billing information.
/// </summary>
/// <param name="BillingId">Global Billing Id.</param>
/// <param name="BillingKind"> Billing Kind.</param>
/// <param name="Currency">Currency Format.</param>
/// <param name="BudgetLimit">Budget Limt.</param>
/// <param name="HostingFee">Hosting Fee.</param>
/// <param name="TargetMargin">Target Margin.</param>
/// <param name="TimeFrame">Billing time frame.</param>
public record UpdateBillingCommand(
    int BillingId,
    string BillingKind,
    string? Currency,
    decimal? BudgetLimit,
    decimal? HostingFee,
    int? TargetMargin,
    TimeFrame? TimeFrame
) : IRequest<GlobalBilling>;

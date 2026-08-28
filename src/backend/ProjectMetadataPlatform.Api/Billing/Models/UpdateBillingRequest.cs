using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Api.Billing.Models;

/// <summary>
/// Request for updating global billing information.
/// </summary>
/// <param name="BillingKind">The kind of billing information.</param>
/// <param name="Currency">Default Currency format.</param>
/// <param name="BudgetLimit">Default Budget Limit.</param>
/// <param name="HostingFee">Default Hosting Fee.</param>
/// <param name="TargetMargin">Default Target Margin.</param>
/// <param name="TimeFrame">Default TimeFrame.</param>
public record UpdateBillingRequest(
    string BillingKind,
    string? Currency,
    decimal? BudgetLimit,
    decimal? HostingFee,
    int? TargetMargin,
    TimeFrame? TimeFrame
);

using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Api.Billing.Models;

/// <summary>
/// Response containing a global billing object.
/// </summary>
/// <param name="Id">Id of the billing object.</param>
/// <param name="BillingKind">The Kind of billing.</param>
/// <param name="Currency">Default Currency format.</param>
/// <param name="BudgetLimit">Default Budget Limit.</param>
/// <param name="HostingFee">Default Hosting Fee.</param>
/// <param name="TargetMargin">Default Target Margin.</param>
/// <param name="TimeFrame">Default TimeFrame.</param>
/// <param name="Permissions">Permissions on the billing information.</param>
public record GetBillingResponse(
    int Id,
    string BillingKind,
    string? Currency,
    decimal? BudgetLimit,
    decimal? HostingFee,
    int? TargetMargin,
    TimeFrame? TimeFrame,
    List<AuthorizationConstants.Actions>? Permissions = null
);

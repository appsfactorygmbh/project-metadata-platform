using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Application.Billing;

/// <summary>
/// Request to create new global billing information.
/// </summary>
/// <param name="BillingKind">Billing kind.</param>
/// <param name="Currency">Currency format.</param>
/// <param name="TargetMargin">Target Margin.</param>
/// <param name="TimeFrame">Billing Time frame.</param>
public record CreateBillingCommand(
    string BillingKind,
    Currencies? Currency,
    int? TargetMargin,
    TimeFrame? TimeFrame
) : IRequest<int>;

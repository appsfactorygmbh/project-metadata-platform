using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Billing;

/// <summary>
/// Request to delete global billing information.
/// </summary>
/// <param name="Id">Id of the billing object.</param>
public record DeleteBillingCommand(int Id) : IRequest;

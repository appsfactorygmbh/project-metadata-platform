namespace ProjectMetadataPlatform.Api.Billing.Models;

/// <summary>
/// Response for deleting billing information.
/// </summary>
/// <param name="GlobalBillingId">The id of the billing information.</param>
public record DeleteBillingResponse(int GlobalBillingId);

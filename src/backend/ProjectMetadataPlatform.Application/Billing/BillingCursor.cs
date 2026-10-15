using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.Billing;

/// <summary>
/// Cursor Record point toward a Global Billing Resource
/// </summary>
/// <param name="BillingKind">Kind of the billing Information</param>
public record BillingCursor(string BillingKind) : Cursor;

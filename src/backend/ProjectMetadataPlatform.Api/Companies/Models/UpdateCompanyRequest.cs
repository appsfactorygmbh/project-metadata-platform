namespace ProjectMetadataPlatform.Api.Companies.Models;

/// <summary>
/// Represents a Request to Update a Company.
/// </summary>
/// <param name="CompanyName">New Company Name.</param>
public record UpdateCompanyRequest(string? CompanyName = null);

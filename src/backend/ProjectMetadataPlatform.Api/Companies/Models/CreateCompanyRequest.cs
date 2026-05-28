namespace ProjectMetadataPlatform.Api.Companies.Models;

/// <summary>
/// Represents a Request to create a Company.
/// </summary>
/// <param name="CompanyName">Name of the New Company. </param>
public record CreateCompanyRequest(string CompanyName);

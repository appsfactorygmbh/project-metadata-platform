using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Cursor Record pointing towards an Company Resource.
/// </summary>
/// <param name="CompanyName">Name of the Company.</param>
public record CompanyCursor(string CompanyName) : Cursor;

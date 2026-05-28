namespace ProjectMetadataPlatform.Api.OfficeLocations.Models;

/// <summary>
/// Represents a Request to create a new Office Location.
/// </summary>
/// <param name="OfficeLocationName">Name of the new Office Location.</param>
public record CreateOfficeLocationRequest(string OfficeLocationName);

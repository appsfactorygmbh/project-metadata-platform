namespace ProjectMetadataPlatform.Api.OfficeLocations.Models;

/// <summary>
/// Represents a Request to update a Office Location.
/// </summary>
/// <param name="OfficeLocationName">New Office Location Name.</param>
public record UpdateOfficeLocationRequest(string? OfficeLocationName = null);

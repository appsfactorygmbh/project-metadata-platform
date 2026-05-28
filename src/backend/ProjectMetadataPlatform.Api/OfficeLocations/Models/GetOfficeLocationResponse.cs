namespace ProjectMetadataPlatform.Api.OfficeLocations.Models;

/// <summary>
/// Represents a Response containing a single Office Location.
/// </summary>
/// <param name="Id">Id of the Office Location. </param>
/// <param name="OfficeLocationName">Name of the Office Location. </param>
public record GetOfficeLocationResponse(int Id, string OfficeLocationName);

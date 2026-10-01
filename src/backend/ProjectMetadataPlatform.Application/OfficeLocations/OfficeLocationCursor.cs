using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Cursor Record pointing towards an Office Location Resource.
/// </summary>
/// <param name="OfficeLocationName">Name of the Office Location.</param>
public record OfficeLocationCursor(string OfficeLocationName) : Cursor;

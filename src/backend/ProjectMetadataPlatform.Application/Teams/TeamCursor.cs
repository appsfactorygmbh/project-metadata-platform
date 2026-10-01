using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.Teams;

/// <summary>
/// Cursor Record pointing towards a Team Resource.
/// </summary>
/// <param name="TeamName">Name of the Team.</param>
public record TeamCursor(string TeamName) : Cursor;

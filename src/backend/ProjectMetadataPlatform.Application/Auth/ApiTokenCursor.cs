using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Cursor pointing towards a ApiToken record.
/// </summary>
/// <param name="Name">Name of the ApiToken</param>
public record ApiTokenCursor(string Name) : Cursor;

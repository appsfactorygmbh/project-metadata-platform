using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Cursor Record pointing towards a Project Resource.
/// </summary>
/// <param name="Slug">Slug of the Project.</param>
/// <param name="CursorValue">Sorting Attribute of the Project: Client Name, Project Name, Company Name, Business Unit Name or Team Name.</param>
public record ProjectCursor(string Slug, string CursorValue) : Cursor;

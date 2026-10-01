using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.Departments;

/// <summary>
/// Cursor Record pointing to a Department Resource.
/// </summary>
/// <param name="DepartmentName">Name of the Department.</param>
public record DepartmentCursor(string DepartmentName) : Cursor;

using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

/// <summary>
/// Exception thrown when a project slug already exists but should be unique.
/// </summary>
/// <param name="slug">The slug that already exists.</param>
public class ProjectSlugAlreadyExistsException(string slug)
    : EntityAlreadyExistsException("A Project with this slug already exists: " + slug);

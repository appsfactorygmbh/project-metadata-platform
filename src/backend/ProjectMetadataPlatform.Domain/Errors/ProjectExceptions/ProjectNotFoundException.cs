using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

/// <summary>
/// Exception thrown when a project is not found in the Project Metadata Platform.
/// </summary>
public class ProjectNotFoundException : EntityNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectNotFoundException"/> class with a specified project ID.
    /// </summary>
    /// <param name="projectId">The ID of the project that was not found.</param>
    public ProjectNotFoundException(int projectId)
        : base("The project with id " + projectId + " was not found.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectNotFoundException"/> class with a specified project slug.
    /// </summary>
    /// <param name="slug">The slug of the project that was not found.</param>
    public ProjectNotFoundException(string slug)
        : base("The project with slug " + slug + " was not found.") { }
}

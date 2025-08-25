using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

/// <summary>
/// Exception thrown when a project is not archived when trying to delete it.
/// </summary>
/// <param name="project">The project that is not archived.</param>
public class ProjectNotArchivedException(Project project)
    : ProjectException("The project " + project.Id + " is not archived.");

namespace ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

/// <summary>
/// Represents an abstract base class for project-related exceptions, used to mark exceptions that are related to projects and need specific error responses.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public abstract class ProjectException(string message) : PmpException(message);

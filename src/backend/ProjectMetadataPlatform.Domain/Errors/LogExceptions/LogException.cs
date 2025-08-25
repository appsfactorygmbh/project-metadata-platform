namespace ProjectMetadataPlatform.Domain.Errors.LogExceptions;

/// <summary>
/// Represents an abstract base class for log-related exceptions in the Project Metadata Platform used to mark exceptions that are related to logs so that they are handled by the ProjectsExceptionHandler.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public abstract class LogException(string message) : PmpException(message);

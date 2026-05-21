namespace ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;

/// <summary>
/// Represents an abstract base class for  Department-related exceptions, used to mark exceptions that are related to  Departments and need specific error responses.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public abstract class DepartmentException(string message) : PmpException(message);

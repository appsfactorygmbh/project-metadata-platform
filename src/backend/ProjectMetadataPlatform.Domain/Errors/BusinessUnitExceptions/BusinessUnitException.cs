namespace ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;

/// <summary>
/// Represents an abstract base class for bu-related exceptions, used to mark exceptions that are related to business units and need specific error responses.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public abstract class BusinessUnitException(string message) : PmpException(message);

namespace ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;

/// <summary>
/// Represents an abstract base class for company-related exceptions, used to mark exceptions that are related to companies and need specific error responses.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public abstract class CompanyException(string message) : PmpException(message);

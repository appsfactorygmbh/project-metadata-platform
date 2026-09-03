namespace ProjectMetadataPlatform.Domain.Errors.BillingExceptions;

/// <summary>
/// Represents an abstract base class for billing-related exceptions, used to mark exceptions that are related to billing and need specific error responses.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public abstract class BillingException(string message) : PmpException(message);

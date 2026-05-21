namespace ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;

/// <summary>
/// Represents an abstract base class for office-location-related exceptions, used to mark exceptions that are related to office locations and need specific error responses.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public abstract class OfficeLocationException(string message) : PmpException(message);

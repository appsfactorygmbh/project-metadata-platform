using System;

namespace ProjectMetadataPlatform.Domain.Errors;

/// <summary>
/// Represents a base exception class for the Project Metadata Platform.
/// </summary>
public abstract class PmpException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PmpException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected PmpException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PmpException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected PmpException(string message, Exception innerException)
        : base(message, innerException) { }
}

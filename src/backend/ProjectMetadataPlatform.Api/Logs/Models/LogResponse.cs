namespace ProjectMetadataPlatform.Api.Logs.Models;

/// <summary>
/// Represents a response containing a log message and its timestamp.
/// </summary>
/// <param name="LogMessage">The message of the log entry.</param>
/// <param name="Timestamp">The timestamp of the log entry.</param>
public record LogResponse(string LogMessage, string Timestamp);

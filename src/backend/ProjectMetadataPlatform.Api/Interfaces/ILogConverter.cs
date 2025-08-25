using ProjectMetadataPlatform.Api.Logs.Models;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Api.Interfaces;

/// <summary>
/// Provides methods to convert log entries into log responses.
/// </summary>
public interface ILogConverter
{
    /// <summary>
    /// Builds a log message from a log entry.
    /// </summary>
    /// <param name="log">The log entry.</param>
    /// <returns>A log response containing the message and timestamp.</returns>
    public LogResponse BuildLogMessage(Log log);
}

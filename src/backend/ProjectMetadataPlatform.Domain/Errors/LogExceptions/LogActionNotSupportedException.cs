using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Domain.Errors.LogExceptions;

/// <summary>
/// Exception thrown when a specific log action is not supported.
/// </summary>
/// <param name="action">The action that is not supported.</param>
/// <param name="logType">The type of log that does not support the action.</param>
public class LogActionNotSupportedException(Action action, string logType)
    : LogException("The action " + action + " is not supported " + "for " + logType + " logs.");

using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ProjectMetadataPlatform.Api.Telemetry;

/// <summary>
/// Marks the current Activity as failed when an error-level log entry is written.
/// </summary>
public sealed class ActivityErrorLoggerProvider : ILoggerProvider
{
    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName) => new ActivityErrorLogger(categoryName);

    /// <inheritdoc />
    public void Dispose() { }

    private sealed class ActivityErrorLogger(string categoryName) : ILogger
    {
        private readonly string _categoryName = categoryName;

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull => NullScope.Instance;

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Error;

        /// <inheritdoc />
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            if (logLevel < LogLevel.Error)
            {
                return;
            }

            var activity = Activity.Current;
            if (activity is null)
            {
                return;
            }

            var message = formatter(state, exception);
            _ = activity.SetStatus(
                ActivityStatusCode.Error,
                CreateStatusDescription(message, exception)
            );
            _ = activity.SetTag("app.log.error", true);
            _ = activity.SetTag("app.log.error_count", GetErrorCount(activity) + 1);
            _ = activity.SetTag("app.log.severity", logLevel.ToString());
            _ = activity.SetTag("app.log.category", _categoryName);

            var tags = new ActivityTagsCollection
            {
                { "log.severity", logLevel.ToString() },
                { "log.category", _categoryName },
            };

            if (!string.IsNullOrWhiteSpace(message))
            {
                tags.Add("log.message", message);
            }

            if (eventId.Id != 0)
            {
                tags.Add("log.event_id", eventId.Id);
            }

            if (!string.IsNullOrWhiteSpace(eventId.Name))
            {
                tags.Add("log.event_name", eventId.Name);
            }

            if (exception is not null)
            {
                tags.Add("exception.type", exception.GetType().FullName);
                tags.Add("exception.message", exception.Message);

                if (!string.IsNullOrWhiteSpace(exception.StackTrace))
                {
                    tags.Add("exception.stacktrace", exception.StackTrace);
                }
            }

            _ = activity.AddEvent(new ActivityEvent("log.error", tags: tags));
        }

        private static string CreateStatusDescription(string? message, Exception? exception)
        {
            var description = !string.IsNullOrWhiteSpace(message) ? message : exception?.Message;
            if (string.IsNullOrWhiteSpace(description))
            {
                return "ILogger reported an error.";
            }

            const int maxLength = 200;
            return description.Length <= maxLength
                ? description
                : string.Concat(description.AsSpan(0, maxLength - 3), "...");
        }

        private static int GetErrorCount(Activity activity)
        {
            return activity.GetTagItem("app.log.error_count") switch
            {
                int count => count,
                long count => checked((int)count),
                string count when int.TryParse(count, out var parsed) => parsed,
                _ => 0,
            };
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose() { }
    }
}

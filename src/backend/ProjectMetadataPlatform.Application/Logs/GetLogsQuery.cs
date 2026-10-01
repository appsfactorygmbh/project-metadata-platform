using System;
using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Logs;

/// <summary>
/// Represents a query to retrieve logs based on project ID and search criteria.
/// </summary>
/// <param name="Cursor">Optional Cursor for pagination.</param>
/// <param name="Limit">Optional Limit for pagination.</param>
/// <param name="StartDate">StartDate to filter logs by.</param>
/// <param name="EndDate">EndDate to filter logs by.</param>
/// <param name="ProjectId">The ID of the project to filter logs by.</param>
/// <param name="Search">The search term to filter logs by.</param>
/// <param name="UserId">The ID of the user to filter logs by.</param>
/// <param name="GlobalPluginId">The ID of the global plugin to filter logs by.</param>
/// <returns>A list of log responses.</returns>
public record GetLogsQuery(
    LogCursor? Cursor,
    int? Limit,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    int? ProjectId = null,
    string? Search = null,
    string? UserId = null,
    int? GlobalPluginId = null
) : IRequest<(IEnumerable<Log>, LogCursor?)>;

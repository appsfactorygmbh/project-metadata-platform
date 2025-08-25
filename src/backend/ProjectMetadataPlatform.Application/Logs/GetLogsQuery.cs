using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Logs;

/// <summary>
/// Represents a query to retrieve logs based on project ID and search criteria.
/// </summary>
/// <param name="ProjectId">The ID of the project to filter logs by.</param>
/// <param name="Search">The search term to filter logs by.</param>
/// <param name="UserId">The ID of the user to filter logs by.</param>
/// <param name="GlobalPluginId">The ID of the global plugin to filter logs by.</param>
/// <returns>A list of log responses.</returns>
public record GetLogsQuery(
    int? ProjectId = null,
    string? Search = null,
    string? UserId = null,
    int? GlobalPluginId = null
) : IRequest<IEnumerable<Log>>;

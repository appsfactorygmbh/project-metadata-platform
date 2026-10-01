using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Teams;

/// <summary>
/// Query to get all projects or all projects with specific search pattern
/// <param name="FullTextQuery">Optional. Full text search over all attributes of a team except the id.</param>
/// <param name="TeamName">Optional. The name of the team to filter by.</param>
/// <param name="Cursor">Optional Cursor for pagination.</param>
/// <param name="Limit">Optional Limit for pagination.</param>
/// </summary>
public record GetAllTeamsQuery(
    string? FullTextQuery,
    string? TeamName,
    TeamCursor? Cursor,
    int? Limit
) : IRequest<(IEnumerable<Team>, IEnumerable<AuthorizationConstants.Actions>, TeamCursor?)>;

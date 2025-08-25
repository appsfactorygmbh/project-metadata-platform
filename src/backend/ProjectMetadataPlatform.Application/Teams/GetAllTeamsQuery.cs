using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Query to get all projects or all projects with specific search pattern
/// <param name="FullTextQuery">Optional. Full text search over all attributes of a team except the id.</param>
/// <param name="TeamName">Optional. The name of the team to filter by.</param>
/// </summary>
public record GetAllTeamsQuery(string? FullTextQuery, string? TeamName)
    : IRequest<IEnumerable<Team>>;

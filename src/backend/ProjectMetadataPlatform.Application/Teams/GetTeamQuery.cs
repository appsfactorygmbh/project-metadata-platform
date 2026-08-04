using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Teams;

/// <summary>
/// Query to get a team by id.
/// </summary>
public record GetTeamQuery(int Id) : IRequest<(Team, IEnumerable<AuthorizationConstants.Actions>)>;

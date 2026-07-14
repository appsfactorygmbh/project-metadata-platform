using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Teams;

/// <summary>
/// Query to get a team by id.
/// </summary>
public record GetTeamQuery(int Id) : IRequest<(Team, IEnumerable<AuthorizationConstants.Actions>)>;

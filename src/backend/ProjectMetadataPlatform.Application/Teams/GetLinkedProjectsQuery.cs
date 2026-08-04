using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Teams;

/// <summary>
/// Query to get linked projects to team by id.
/// </summary>
public record GetLinkedProjectsQuery(int Id) : IRequest<List<string>>;

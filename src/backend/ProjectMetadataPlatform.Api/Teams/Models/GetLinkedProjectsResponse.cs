using System.Collections.Generic;

namespace ProjectMetadataPlatform.Api.Teams.Models;

/// <summary>
/// Response for receiving the linked projects for a team.
/// </summary>
/// <param name="ProjectSlugs">A list of project slugs the team is linked to.</param>
public record GetLinkedProjectsResponse(List<string> ProjectSlugs);

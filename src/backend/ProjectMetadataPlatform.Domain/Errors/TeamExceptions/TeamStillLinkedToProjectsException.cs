using System;
using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

/// <summary>
/// Exception thrown when a project is not archived when trying to delete it.
/// </summary>
/// <param name="team">The team that cant be deleted.</param>
/// <param name="projectIds"></param>
public class TeamStillLinkedToProjectsException(Team team, List<int> projectIds)
    : TeamException(
        $"The team {team.Id} ({team.TeamName}) cant be deleted because it is still linked to these projects (ids): [{string.Join(", ", projectIds)}]"
    );

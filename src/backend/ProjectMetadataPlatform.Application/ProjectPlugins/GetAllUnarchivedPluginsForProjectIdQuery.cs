using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.ProjectPlugins.Models;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.ProjectPlugins;

/// <summary>
/// Query to get all unarchived plugins for a given project id.
/// </summary>
/// <param name="Id">selects the project</param>
public record GetAllUnarchivedPluginsForProjectIdQuery(int Id)
    : IRequest<(
        IEnumerable<ProjectPluginPermissionModel>,
        IEnumerable<AuthorizationConstants.Actions>
    )>;

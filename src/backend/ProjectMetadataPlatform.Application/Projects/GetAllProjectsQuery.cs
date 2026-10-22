using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Query to get all projects or all projects with specific search pattern
/// <param name="Request">The collection of filters to search by.</param>
/// <param name="Search">Search string to filter the projects by.</param>
/// <param name="Cursor">Optional Cursor for pagination.</param>
/// <param name="Limit">Optional Limit for pagination.</param>
/// </summary>
public record GetAllProjectsQuery(
    ProjectFilterRequest? Request,
    string? Search,
    ProjectCursor? Cursor,
    int? Limit
) : IRequest<(IEnumerable<Project>, IEnumerable<AuthorizationConstants.Actions>, ProjectCursor?)>;

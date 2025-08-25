using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Query to get all projects or all projects with specific search pattern
/// <param name="Request">The collection of filters to search by.</param>
/// <param name="Search">Search string to filter the projects by.</param>
/// </summary>
public record GetAllProjectsQuery(ProjectFilterRequest? Request, string? Search)
    : IRequest<IEnumerable<Project>>;

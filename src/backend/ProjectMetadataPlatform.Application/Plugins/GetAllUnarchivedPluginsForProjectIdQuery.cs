using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Query to get all unarchived plugins for a given project id.
/// </summary>
/// <param name="Id">selects the project</param>
public record GetAllUnarchivedPluginsForProjectIdQuery(int Id) : IRequest<List<ProjectPlugins>>;

using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Command to delete a project by its Id.
/// </summary>
/// <param name="Id">The Id of the project to delete.</param>
/// <returns>The deleted project.</returns>
public record DeleteProjectCommand(int Id) : IRequest<Project?>;

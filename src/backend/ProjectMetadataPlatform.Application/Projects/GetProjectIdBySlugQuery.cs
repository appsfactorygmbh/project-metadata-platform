using MediatR;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Query for getting the Id of a project by its slug.
/// <param name="Slug">The slug of the project to get the Id for.</param>
/// </summary>
public record GetProjectIdBySlugQuery(string Slug) : IRequest<int>;

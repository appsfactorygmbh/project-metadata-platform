using MediatR;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Query to get a project by id.
/// </summary>
public record GetProjectQuery(int Id) : IRequest<Project>;

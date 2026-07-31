using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Query to get a project by id.
/// </summary>
public record GetProjectQuery(int Id)
    : IRequest<(Project, IEnumerable<AuthorizationConstants.Actions>)>;

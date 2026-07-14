using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Query to retrieve all projects with a scim filter.
/// </summary>
public record GetAllUsersQuery(string Filter)
    : IRequest<(IEnumerable<ApplicationUser>, IEnumerable<AuthorizationConstants.Actions>)>;

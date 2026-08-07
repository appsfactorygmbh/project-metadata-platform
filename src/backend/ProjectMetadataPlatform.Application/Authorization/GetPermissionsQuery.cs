using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Query for getting permissions on a Resource.
/// </summary>
/// <param name="ResourceKind">Name of the Resource type.</param>
public record GetPermissionsQuery(string ResourceKind)
    : IRequest<Dictionary<AuthorizationConstants.Actions, string>>;

using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Authorization.Models;

/// <summary>
/// Record representing a Permission.
/// </summary>
/// <param name="Action">Action on the Resource.</param>
/// <param name="Filter"> Tree structure representing the Access Filter.</param>
public record GetPermissionResponse(
    AuthorizationConstants.Actions Action,
    GetFilterResponse Filter
);

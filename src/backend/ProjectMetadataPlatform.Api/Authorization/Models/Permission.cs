using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Authorization.Models;

/// <summary>
/// Record representing a Permission.
/// </summary>
/// <param name="Action">Action on the Resource.</param>
/// <param name="Filter">Human readable filter representing conditions for accessing the resource.</param>
public record ActionPermissionFilter(AuthorizationConstants.Actions Action, string Filter);

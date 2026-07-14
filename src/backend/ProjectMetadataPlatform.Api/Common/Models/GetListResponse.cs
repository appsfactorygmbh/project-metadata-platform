using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Common.Models;

/// <summary>
/// Response for Getting Multiple Resources.
/// </summary>
/// <typeparam name="T">Resource Type.</typeparam>
/// <param name="Resources">List of Resources.</param>
/// <param name="Permissions">Permissions on the Resource type.</param>
public record GetListResponse<T>(
    List<T> Resources,
    List<AuthorizationConstants.Actions> Permissions
);

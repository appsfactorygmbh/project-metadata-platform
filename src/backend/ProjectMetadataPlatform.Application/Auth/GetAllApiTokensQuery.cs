using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Query for getting all Api tokens.
/// </summary>
public record GetAllApiTokensQuery(ApiTokenCursor? Cursor, int? Limit)
    : IRequest<(
        IEnumerable<ApiToken>,
        IEnumerable<AuthorizationConstants.Actions>,
        ApiTokenCursor?
    )>;

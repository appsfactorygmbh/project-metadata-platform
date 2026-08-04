using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Query for getting the details of a token.
/// </summary>
/// <param name="TokenId">Id of the token</param>
public record GetApiTokenDetailsQuery(int TokenId)
    : IRequest<(ApiToken, IEnumerable<AuthorizationConstants.Actions>)>;

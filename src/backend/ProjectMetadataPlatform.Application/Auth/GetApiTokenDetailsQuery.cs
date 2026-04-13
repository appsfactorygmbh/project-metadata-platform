using System.Collections;
using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Query for getting the details of a token.
/// </summary>
/// <param name="TokenId">Id of the token</param>
public record GetApiTokenDetailsQuery(int TokenId) : IRequest<ApiToken>;

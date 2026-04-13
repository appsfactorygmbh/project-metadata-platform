using System.Collections;
using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Query for getting all Api tokens.
/// </summary>
public record GetAllApiTokensQuery() : IRequest<IEnumerable<ApiToken>>;

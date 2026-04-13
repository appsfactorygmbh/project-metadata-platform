using System.Collections;
using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Command for regenerating an Api token value.
/// </summary>
/// <param name="TokenId">Id of the token.</param>
public record RegenerateApiTokenCommand(int TokenId) : IRequest<ApiToken>;


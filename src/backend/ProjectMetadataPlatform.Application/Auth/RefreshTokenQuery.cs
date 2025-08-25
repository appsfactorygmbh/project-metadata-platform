using MediatR;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Query for refreshing an authentication token.
/// </summary>
/// <param name="RefreshToken">a refresh Token</param>
public record RefreshTokenQuery(string RefreshToken) : IRequest<JwtTokens>;

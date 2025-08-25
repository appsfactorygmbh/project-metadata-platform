using MediatR;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Query for logging in.
/// </summary>
/// <param name="Email">Username of the user</param>
/// <param name="Password">Password of the user</param>
public record LoginQuery(string Email, string Password) : IRequest<JwtTokens>;

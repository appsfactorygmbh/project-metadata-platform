using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Query to get a user by id.
/// </summary>
public record GetUserQuery(string UserId) : IRequest<IdentityUser>;

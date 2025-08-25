using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Query to retrieve all projects.
/// </summary>
public record GetAllUsersQuery : IRequest<IEnumerable<IdentityUser>>;

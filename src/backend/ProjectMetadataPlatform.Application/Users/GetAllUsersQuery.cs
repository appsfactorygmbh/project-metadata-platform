using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Query to retrieve all projects.
/// </summary>
public record GetAllUsersQuery(string Filter) : IRequest<IEnumerable<ApplicationUser>>;

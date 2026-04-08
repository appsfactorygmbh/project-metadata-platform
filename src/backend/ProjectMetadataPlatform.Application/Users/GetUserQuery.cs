using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Query to get a user by id.
/// </summary>
public record GetUserQuery(string EmployeeId) : IRequest<ApplicationUser>;

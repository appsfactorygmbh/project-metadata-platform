using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Command to create a new user.
/// </summary>
/// <param name="Email">Email of the user.</param>
/// <param name="Password">Password of the user.</param>
public record CreateUserCommand(
    string EmployeeId,
    string Email,
    string? Password,
    bool? IsActive,
    bool? IsScimProvisioned,
    List<string>? Teams,
    List<string>? TeamSupport,
    List<string>? BusinessUnits,
    List<string>? JobTitles,
    List<string>? Departments,
    string? Company
) : IRequest<ApplicationUser>;

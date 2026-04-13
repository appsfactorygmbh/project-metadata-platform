using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Command to create a User.
/// </summary>
/// <param name="EmployeeId">Employee Id of the user.</param>
/// <param name="Email">Email of the user.</param>
/// <param name="Password">Password of the user.</param>
/// <param name="IsActive">Activity status of the user.</param>
/// <param name="IsScimProvisioned">Wether the user was created via api token or manually</param>
/// <param name="Teams"> List of teamnames of teams the user belongs to.</param>
/// <param name="TeamSupport">List of teamnames of teams the user is supporting.</param>
/// <param name="BusinessUnits">List of BUs the user belongs to.</param>
/// <param name="JobTitles">List of jobtitles of the user.</param>
/// <param name="Departments">List of departments the user belongs to.</param>
/// <param name="Company">Name of the Company the user belongs to.</param>
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

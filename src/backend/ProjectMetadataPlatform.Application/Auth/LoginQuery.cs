using MediatR;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Query for logging in.
/// </summary>
/// <param name="Email">Username of the user</param>
/// <param name="Password">Password of the user</param>
/// <param name="JobTitle">Comma Separated List of JobTitles of the User</param>
/// <param name="Department">Comma Separated List of Departments, Teams and BUs of the User</param>
/// <param name="TeamSupport">Comma Separated List of Teams the User is a Supporter on</param>
/// <param name="Company">Company of the User</param>
public record LoginQuery(
    string Email,
    string Password,
    string JobTitle = "",
    string Department = "",
    string TeamSupport = "",
    string Company = ""
) : IRequest<JwtTokens>;

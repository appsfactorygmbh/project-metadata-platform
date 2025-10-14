using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Suject requesting access to a ressource.
/// </summary>
public class AuthorizationSubject
{
    /// <summary>
    /// Email adress of the subject. Identifier.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// List of special Jobtitles of the Subject.
    /// </summary>
    public IEnumerable<string> JobTitle { get; }

    /// <summary>
    /// List of Business Units the subject belongs to.
    /// </summary>
    public IEnumerable<string> BusinessUnits { get; }

    /// <summary>
    /// List of Departments the subject belongs to.
    /// </summary>
    public IEnumerable<string> Departments { get; }

    /// <summary>
    /// List of Teams the subject belongs to.
    /// </summary>
    public IEnumerable<string> Teams { get; }

    /// <summary>
    /// List of Teams the subject is an supporter on.
    /// </summary>
    public IEnumerable<string> TeamSupport { get; }

    /// <summary>
    /// Company the subject belongs to.
    /// </summary>
    public string? Company { get; }

    private AuthorizationSubject(
        string email,
        IEnumerable<string> employeeType,
        IEnumerable<string> jobTitle,
        IEnumerable<string> departments,
        IEnumerable<string> teams,
        IEnumerable<string> teamSupport,
        string? company
    )
    {
        Email = email;
        JobTitle = employeeType;
        BusinessUnits = jobTitle;
        Departments = departments;
        Teams = teams;
        TeamSupport = teamSupport;
        Company = company;
    }

    /// <summary>
    /// Converts a ClaimsPrincipal to an Authorization Subject.
    /// </summary>
    /// <param name="user">Claims Principal for a Subject.</param>
    /// <returns>Authorization Subject</returns>
    public static AuthorizationSubject ConvertClaimsToAuthorizationSubject(ClaimsPrincipal user)
    {
        var email = user.FindFirstValue(ClaimTypes.Email)!;

        var jobTitle =
            user.FindFirstValue("JobTitle")
                ?.Split(',')
                .Select(x => x.Replace("[", "").Replace("]", "").Trim()) ?? [];

        var departmentsBUsTeams =
            user.FindFirstValue("Department")
                ?.Split(',')
                .Select(x => x.Replace("[", "").Replace("]", "").Trim()) ?? [];

        var buisinessUnits = departmentsBUsTeams.Where(x => x.Contains("BU"));

        var departments = departmentsBUsTeams.Where(x => !x.Contains('#') && !x.Contains("BU"));

        var teams = departmentsBUsTeams.Where(x => x.Contains('#'));

        var teamSupport =
            user.FindFirstValue("TeamSupport")
                ?.Split(',')
                .Select(x => x.Replace("[", "").Replace("]", "").Trim()) ?? [];

        var company = user.FindFirstValue("Company");

        return new AuthorizationSubject(
            email,
            jobTitle,
            buisinessUnits,
            departments,
            teams,
            teamSupport,
            company
        );
    }
}

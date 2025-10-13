using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ProjectMetadataPlatform.Application.Authorization;

public class AuthorizationSubject
{
    public string Email { get; }

    public IEnumerable<string> JobTitle { get; }

    public IEnumerable<string> BusinessUnits { get; }

    public IEnumerable<string> Departments { get; }

    public IEnumerable<string> Teams { get; }

    public IEnumerable<string> TeamSupport { get; }

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

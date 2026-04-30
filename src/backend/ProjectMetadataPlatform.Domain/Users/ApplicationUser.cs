using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Domain.Users;

/// <summary>
/// Class representing an ApplicationUser in the database
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Employee Number of the user.
    /// </summary>
    public required string EmployeeId { get; set; }

    /// <summary>
    /// Holds the relation between Users and Teams.
    /// </summary>
    public ICollection<Team>? Teams { get; set; }

    /// <summary>
    /// Holds the relation between Users and Teams they support.
    /// </summary>
    public ICollection<Team>? TeamSupport { get; set; }

    /// <summary>
    /// Signals wether the user is active or not.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Signals wether the user was scimprovisioned or not.
    /// </summary>
    public bool IsScimProvisioned { get; set; }

    /// <summary>
    /// List of BUs of the user.
    /// </summary>
    public List<string>? BusinessUnits { get; set; }

    /// <summary>
    /// List of jobtitles of the user.
    /// </summary>
    public List<string>? JobTitles { get; set; }

    /// <summary>
    /// List of departments the user belongs to.
    /// </summary>
    public List<string>? Departments { get; set; }

    /// <summary>
    /// Name of the company the user belongs to.
    /// </summary>
    public string? Company { get; set; }
}

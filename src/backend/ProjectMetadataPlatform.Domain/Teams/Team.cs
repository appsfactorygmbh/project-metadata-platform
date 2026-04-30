using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.Teams;

/// <summary>
/// The representation of a team in the Database.
/// </summary>
public class Team
{
    /// <summary>
    /// Gets or sets the id of the team.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the team. This property is required and unique.
    /// </summary>
    public required string TeamName { get; set; }

    /// <summary>
    /// Gets or sets the business unit associated with the team. This property is required.
    /// </summary>
    public required string BusinessUnit { get; set; }

    /// <summary>
    /// Gets or sets the PTL associated with the team.
    /// </summary>
    public string? PTL { get; set; }

    /// <summary>
    /// Holds the relation between Projects and Teams.
    /// </summary>
    public ICollection<Project>? Projects { get; set; }

    /// <summary>
    /// Holds the relation between Users and Teams.
    /// </summary>
    public ICollection<ApplicationUser>? Users { get; set; }

    /// <summary>
    /// Holds the relation between Users and Teams they support.
    /// </summary>
    public ICollection<ApplicationUser>? TeamSupportUsers { get; set; }
}

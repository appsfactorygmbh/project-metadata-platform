using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.BusinessUnits;

/// <summary>
/// Representation of a Business Unit in the database.
/// </summary>
public class BusinessUnit
{
    /// <summary>
    /// Id of the Business Unit.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the Business Unit. Required and Unique.
    /// </summary>
    public required string BusinessUnitName { get; set; }

    /// <summary>
    /// Holds the relation between Teams and Business Units.
    /// </summary>
    public ICollection<Team>? Teams { get; set; }

    /// <summary>
    /// Holds the relation between Users and Business Units.
    /// </summary>
    public ICollection<ApplicationUser>? Users { get; set; }
}

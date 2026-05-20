using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.BusinessUnits;

public class BusinessUnit
{
    public int Id { get; set; }

    public required string BusinessUnitName { get; set; }

    /// <summary>
    /// Holds the relation between Teams and Business Units.
    /// </summary>
    public ICollection<Team>? Teams { get; set; }

    public ICollection<ApplicationUser>? Users { get; set; }
}

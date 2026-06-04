using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.OfficeLocations;

/// <summary>
/// Represents an Office Location in the database.
/// </summary>
public class OfficeLocation
{
    /// <summary>
    /// Id of the office location.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the office location. Required and Unique.
    /// </summary>
    public required string OfficeLocationName { get; set; }

    /// <summary>
    /// Represents Relation between Users and Office Locations.
    /// </summary>
    public ICollection<ApplicationUser>? Users { get; set; }
}

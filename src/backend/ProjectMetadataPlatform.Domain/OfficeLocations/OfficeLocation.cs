using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.OfficeLocations;

public class OfficeLocation
{
    public int Id { get; set; }

    public required string OfficeLocationName { get; set; }

    public ICollection<ApplicationUser>? Users { get; set; }
}

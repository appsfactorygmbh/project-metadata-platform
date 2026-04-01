using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Domain.Users;

public class ApplicationUser : IdentityUser
{
    public ICollection<Team>? Teams { get; set; }

    public ICollection<Team>? TeamSupport { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsScimProvisioned { get; set; }

    public List<string>? BusinessUnits { get; set; }

    public List<string>? JobTitles { get; set; }

    public List<string>? Departments { get; set; }

    public string? Company { get; set; }
}

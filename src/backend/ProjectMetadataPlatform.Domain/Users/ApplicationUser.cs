using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Domain.Users;

public class ApplicationUser : IdentityUser
{
    public Team? Team { get; set; }

    public int? TeamId { get; set; }

    public ICollection<Team>? TeamSupport { get; set; }

    public required bool IsActive { get; set; }

    public required bool IsScimProvisioned { get; set; }

    public List<string> BusinessUnits { get; set; } = [];

    public List<string> JobTitles { get; set; } = [];

    public List<string> Departments { get; set; } = [];

    public string? Company { get; set; }
}

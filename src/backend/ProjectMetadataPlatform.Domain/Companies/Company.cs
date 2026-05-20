using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.Companies;

public class Company
{
    public int Id { get; set; }

    public required string CompanyName { get; set; }

    public ICollection<Project>? Projects { get; set; }

    public ICollection<ApplicationUser>? Users { get; set; }
}

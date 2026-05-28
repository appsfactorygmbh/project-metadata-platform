using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.Companies;

/// <summary>
/// Represents a Company in the Database.
/// </summary>
public class Company
{
    /// <summary>
    /// Id of the Company.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the Company. Unqiue and Required.
    /// </summary>
    public required string CompanyName { get; set; }

    /// <summary>
    /// Holds the relation between Projects and Companies.
    /// </summary>
    public ICollection<Project>? Projects { get; set; }

    /// <summary>
    /// Holds the relation between Users and Companies.
    /// </summary>
    public ICollection<ApplicationUser>? Users { get; set; }
}

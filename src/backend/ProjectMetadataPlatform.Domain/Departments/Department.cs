using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.Departments;

/// <summary>
/// Represents a Department in the database.
/// </summary>
public class Department
{
    /// <summary>
    /// Id of the department.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the Department. Unique and Required.
    /// </summary>
    public required string DepartmentName { get; set; }

    /// <summary>
    /// Holds the relation between Users and Departments.
    /// </summary>
    public ICollection<ApplicationUser>? Users { get; set; }
}

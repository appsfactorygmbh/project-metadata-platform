using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.Departments;

public class Department
{
    public int Id { get; set; }

    public required string DepartmentName { get; set; }

    public ICollection<ApplicationUser>? Users { get; set; }
}

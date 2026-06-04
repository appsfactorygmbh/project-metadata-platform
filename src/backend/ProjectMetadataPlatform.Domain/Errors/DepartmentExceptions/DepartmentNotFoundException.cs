using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;

/// <summary>
/// Exception thrown when a department is not found.
/// </summary>
public class DepartmentNotFoundException : EntityNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the <see cref=" DepartmentNotFoundException"/> class with a specified department ID.
    /// </summary>
    /// <param name="departmentId">The ID of the department that was not found.</param>
    public DepartmentNotFoundException(int departmentId)
        : base("The Department with id " + departmentId + " was not found.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref=" DepartmentNotFoundException"/> class with a specified department name.
    /// </summary>
    /// <param name="department">The name of the Department that was not found.</param>
    public DepartmentNotFoundException(string department)
        : base("The Deparment with name " + department + " was not found.") { }
}

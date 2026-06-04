using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;

/// <summary>
/// Exception thrown when a Department name already exists in the Project Metadata Platform.
/// </summary>
/// <param name="name">The name that already exists.</param>
public class DepartmentNameAlreadyExistsException(string name)
    : EntityAlreadyExistsException("A Department with the name " + name + " already exists.");

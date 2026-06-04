using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;

/// <summary>
/// Exception thrown when a Office Location name already exists in the Project Metadata Platform.
/// </summary>
/// <param name="name">The name that already exists.</param>
public class OfficeLocationNameAlreadyExistsException(string name)
    : EntityAlreadyExistsException("A Office Location with the name " + name + " already exists.");

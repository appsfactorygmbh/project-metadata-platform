using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;

/// <summary>
/// Exception thrown when a BusinessUnit name already exists in the Project Metadata Platform.
/// </summary>
/// <param name="name">The name that already exists.</param>
public class BusinessUnitNameAlreadyExistsException(string name)
    : EntityAlreadyExistsException("A Business Unit with the name " + name + " already exists.");

using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;

/// <summary>
/// Exception thrown when a Company name already exists in the Project Metadata Platform.
/// </summary>
/// <param name="name">The name that already exists.</param>
public class CompanyNameAlreadyExistsException(string name)
    : EntityAlreadyExistsException("A Company with the name " + name + " already exists.");

using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.TeamExceptions;

/// <summary>
/// Exception thrown when a plugin name already exists in the Project Metadata Platform.
/// </summary>
/// <param name="name">The name that already exists.</param>
public class TeamNameAlreadyExistsException(string name)
    : EntityAlreadyExistsException("A Team with the name " + name + " already exists.");

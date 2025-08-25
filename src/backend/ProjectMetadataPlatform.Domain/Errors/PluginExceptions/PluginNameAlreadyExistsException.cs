using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

/// <summary>
/// Exception thrown when a plugin name already exists in the Project Metadata Platform.
/// </summary>
/// <param name="name">The name that already exists.</param>
public class PluginNameAlreadyExistsException(string name)
    : EntityAlreadyExistsException("A global Plugin with the name " + name + " already exists.");

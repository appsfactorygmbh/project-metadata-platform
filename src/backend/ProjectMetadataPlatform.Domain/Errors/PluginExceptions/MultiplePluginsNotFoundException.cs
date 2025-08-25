using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

/// <summary>
/// Exception thrown when multiple plugins are not found.
/// </summary>
/// <param name="pluginIds"></param>
public class MultiplePluginsNotFoundException(List<int> pluginIds)
    : EntityNotFoundException(
        "The Plugins with these ids do not exist: " + string.Join(", ", pluginIds)
    );

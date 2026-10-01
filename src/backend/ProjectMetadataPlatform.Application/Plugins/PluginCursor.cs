using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Cursor Record pointing towards a Global Plugin Record.
/// </summary>
/// <param name="PluginName">Name of the global Plugin.</param>
public record PluginCursor(string PluginName) : Cursor;

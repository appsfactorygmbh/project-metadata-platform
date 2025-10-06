using System;
using System.IO;

namespace ProjectMetadataPlatform.Application;

/// <summary>
///    Utility class for environment related operations.
/// </summary>
public static class EnvironmentUtils
{
    /// <summary>
    /// Gets the value of an environment variable or loads it from the file with the path in environment variable with the name of the given variable plus the '_FILE' suffix.
    /// </summary>
    public static string GetEnvVarOrLoadFromFile(string envVarName)
    {
        var value = Environment.GetEnvironmentVariable(envVarName);

        if (value is not null)
        {
            return value;
        }

        var path =
            Environment.GetEnvironmentVariable(envVarName + "_FILE")
            ?? throw new InvalidOperationException(
                $"Either {envVarName} or {envVarName}_FILE must be configured"
            );

        return File.ReadAllText(path);
    }

    /// <summary>
    /// Replaces Placeholder Strings in the static files of the frontend with their actual value taken from the environment.
    /// </summary>
    public static void AddEnvToStaticFiles()
    {
        string[] placeholders = ["AZURE_FRONTEND_CLIENT_ID", "AZURE_AUTHORITY", "AZURE_SCOPE"];
        var path = "wwwroot/assets/";
        var assets = Directory.GetFiles(path);
        foreach (var file in assets)
        {
            var text = File.ReadAllText(file);
            foreach (var placeholder in placeholders)
            {
                text = text.Replace(placeholder, GetEnvVarOrLoadFromFile(placeholder));
            }
            File.WriteAllText(file, text);
        }
    }
}

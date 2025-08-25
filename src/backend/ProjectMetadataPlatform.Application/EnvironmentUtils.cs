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
}

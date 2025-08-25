namespace ProjectMetadataPlatform.Domain.Projects;

/// <summary>
/// Enum indicating the security level of a project. The higher the security level, the more sensitive the project is.
/// </summary>
public enum SecurityLevel
{
    /// <summary>
    /// Represents a normal security level.
    /// </summary>
    NORMAL,

    /// <summary>
    /// Represents a high security level.
    /// </summary>
    HIGH,

    /// <summary>
    /// Represents a very high security level.
    /// </summary>
    VERY_HIGH,
}

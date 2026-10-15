namespace ProjectMetadataPlatform.Domain.Projects;

/// <summary>
/// Enum indicating if a company is internal or external.
/// </summary>
public enum CompanyState
{
    /// <summary>
    /// Represents an external company.
    /// </summary>
    EXTERNAL = 0,

    /// <summary>
    /// Represents an internal company.
    /// </summary>
    INTERNAL = 1,
}

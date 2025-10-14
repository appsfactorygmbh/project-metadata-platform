namespace ProjectMetadataPlatform.Domain.Authorization;

/// <summary>
/// Effects of a policy result.
/// </summary>
public enum Effect
{
    /// <summary>
    /// access is allowed.
    /// </summary>
    ALLOW,

    /// <summary>
    /// access is denied.
    /// </summary>
    DENY,
}

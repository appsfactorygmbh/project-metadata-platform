namespace ProjectMetadataPlatform.Domain.Billing;

/// <summary>
/// Time frame on which a plugin is billed.
/// </summary>
public enum TimeFrame
{
    /// <summary>
    /// Plugin is billed monthly.
    /// </summary>
    MONTHLY,

    /// <summary>
    /// Plugin is billed quarterly.
    /// </summary>
    QUARTERLY,

    /// <summary>
    /// Plugin is billed yearly.
    /// </summary>
    YEARLY,

    /// <summary>
    /// Plugin is billed on a specific date.
    /// </summary>
    DATE,

    /// <summary>
    /// Plugin is never billed.
    /// </summary>
    NEVER,
}

namespace ProjectMetadataPlatform.Domain.Billing;

/// <summary>
/// Time frame on which a plugin is billed.
/// </summary>
///! Stored as Integers so make sure not to change values without migration steps.
public enum TimeFrame
{
    /// <summary>
    /// Plugin is billed monthly.
    /// </summary>
    MONTHLY = 0,

    /// <summary>
    /// Plugin is billed quarterly.
    /// </summary>
    QUARTERLY = 1,

    /// <summary>
    /// Plugin is billed yearly.
    /// </summary>
    YEARLY = 2,

    /// <summary>
    /// Plugin is billed on a specific date.
    /// </summary>
    DATE = 3,

    /// <summary>
    /// Plugin is never billed.
    /// </summary>
    NEVER = 4,
}

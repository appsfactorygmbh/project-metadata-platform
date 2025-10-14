namespace ProjectMetadataPlatform.Domain.Authorization;

/// <summary>
/// Operation for a policy rule.
/// </summary>
public enum Operation
{
    /// <summary>
    /// Check for Equality.
    /// </summary>
    EQUAL,

    /// <summary>
    /// Check for Inequality.
    /// </summary>
    UNEQUAL,

    /// <summary>
    /// Checks if value is in list.
    /// </summary>
    CONTAINS,

    /// <summary>
    /// Checks if value is not in list.
    /// </summary>
    NOT_IN,

    /// <summary>
    /// Checks if string starts with value.
    /// </summary>
    STARTS_WITH,

    /// <summary>
    /// Checks if string ends with value.
    /// </summary>
    ENDS_WITH,

    /// <summary>
    /// Checks if list or string is empty.
    /// </summary>
    EMPTY,
}

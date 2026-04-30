namespace ProjectMetadataPlatform.Domain.Users;

/// <summary>
/// Enum representing operations that are part of a patch request.
/// </summary>
public enum PatchOperations
{
    /// <summary>
    /// Replaces an existing value.
    /// </summary>
    Replace,

    /// <summary>
    /// Removes an existing value.
    /// </summary>
    Remove,

    /// <summary>
    /// Adds a new value.
    /// </summary>
    Add,
}

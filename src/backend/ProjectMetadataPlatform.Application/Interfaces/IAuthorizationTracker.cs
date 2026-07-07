namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Tracker for Authorization Status
/// </summary>
public interface IAuthorizationTracker
{
    /// <summary>
    /// Whether Authorization was checked or not.
    /// </summary>
    bool WasChecked { get; }

    /// <summary>
    /// Marks Authorization as checked.
    /// </summary>
    void MarkAsChecked();

    /// <summary>
    /// Reverts Authorization check.
    /// </summary>
    void RevertCheck();
}

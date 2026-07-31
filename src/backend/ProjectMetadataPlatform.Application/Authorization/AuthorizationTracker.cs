using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Implements <see cref="IAuthorizationTracker"/>
/// </summary>
public class AuthorizationTracker : IAuthorizationTracker
{
    /// <inheritdoc/>
    public bool WasChecked { get; private set; }

    /// <inheritdoc/>
    public void MarkAsChecked()
    {
        WasChecked = true;
    }

    /// <inheritdoc/>
    public void RevertCheck()
    {
        WasChecked = false;
    }
}

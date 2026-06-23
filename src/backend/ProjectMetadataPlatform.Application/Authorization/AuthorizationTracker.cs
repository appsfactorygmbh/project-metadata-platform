using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Authorization;

public class AuthorizationTracker : IAuthorizationTracker
{
    public bool WasChecked { get; private set; }

    public void MarkAsChecked()
    {
        WasChecked = true;
    }
}

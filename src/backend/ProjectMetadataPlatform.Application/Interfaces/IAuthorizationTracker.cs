namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IAuthorizationTracker
{
    bool WasChecked { get; }
    void MarkAsChecked();
}

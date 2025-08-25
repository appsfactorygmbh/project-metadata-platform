namespace ProjectMetadataPlatform.Domain.Errors.UserException;

/// <summary>
/// Exception thrown when a user attempts to delete themselves.
/// </summary>
public class UserCantDeleteThemselfException : UserException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserCantDeleteThemselfException"/> class with a default error message.
    /// </summary>
    public UserCantDeleteThemselfException()
        : base("A User can't delete themself.") { }
}

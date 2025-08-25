namespace ProjectMetadataPlatform.Domain.Errors.UserException;

/// <summary>
/// Exception thrown when a user is not authenticated.
/// </summary>
public class UserUnauthorizedException : UserException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserUnauthorizedException"/> class.
    /// </summary>
    public UserUnauthorizedException()
        : base("User not authenticated.") { }
}

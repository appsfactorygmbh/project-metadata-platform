using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.UserException;

/// <summary>
/// Exception thrown when a user already exists in the system.
/// </summary>
public class UserAlreadyExistsException : EntityAlreadyExistsException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class.
    /// </summary>
    public UserAlreadyExistsException()
        : base("User creation Failed : DuplicateEmail") { }
}

namespace ProjectMetadataPlatform.Api.Users.Models;

/// <summary>
/// Represents a response to create a new user.
/// </summary>
/// <param name="UserId">Id of the user that was created</param>
public record CreateUserResponse(string UserId);

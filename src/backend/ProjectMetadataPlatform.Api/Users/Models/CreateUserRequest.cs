namespace ProjectMetadataPlatform.Api.Users.Models;

/// <summary>
/// Represents the request to create a new user.
/// </summary>
/// <param name="Email">Email address of a user</param>
/// <param name="Password">Password of a user</param>
public record CreateUserRequest(string Email, string Password);

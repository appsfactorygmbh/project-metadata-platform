using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for user data.
/// </summary>
public interface IUsersRepository
{
    /// <summary>
    /// Creates a new user with the given data.
    /// </summary>
    /// <param name="user">User to be created.</param>
    /// <param name="password">Password of the user.</param>
    /// <returns>Id of the created user.</returns>
    Task<string> CreateUserAsync(ApplicationUser user, string? password);

    /// <summary>
    /// Returns all users.
    /// </summary>
    /// <returns>Enumerable of all User-Objects</returns>
    Task<IEnumerable<ApplicationUser>> GetUsersAsync(string filter);

    /// <summary>
    /// Returns a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The User object if found; otherwise, null.</returns>
    Task<ApplicationUser> GetUserByIdAsync(string id);

    /// <summary>
    /// Returns the user with the given email.
    /// </summary>
    /// <param name="email">The email of the searched for user.</param>
    /// <returns>The user that is searched for or null.</returns>
    Task<ApplicationUser> GetUserByEmailAsync(string email);

    /// <summary>
    /// Stores a user.
    /// </summary>
    /// <param name="user">The User object to store.</param>
    /// <returns>The stored User object.</returns>
    Task<ApplicationUser> StoreUser(ApplicationUser user);

    /// <summary>
    /// Deletes the specified user.
    /// </summary>
    /// <param name="user">The user to be deleted.</param>
    /// <returns>The deleted user.</returns>
    Task<ApplicationUser> DeleteUserAsync(ApplicationUser user);

    /// <summary>
    /// Checks if the given password is in the correct format.
    /// </summary>
    /// <param name="password">password to be checked</param>
    /// <returns>true if the format is correct. Otherwise throws exception</returns>
    Task<bool> CheckPasswordFormat(string password);

    Task<bool> CheckUserExists(string id);
}

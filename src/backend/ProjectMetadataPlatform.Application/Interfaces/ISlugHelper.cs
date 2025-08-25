using System;
using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Provides methods to generate slugs and get Ids from slugs.
/// </summary>
public interface ISlugHelper
{
    /// <summary>
    /// Generates a slug from a string.
    /// </summary>
    /// <param name="input">The string to generate a slug from.</param>
    /// <returns>The generated slug.</returns>
    public string GenerateSlug(string input);

    /// <summary>
    /// Retrieves a projects Id by its slug. When no project is found an exception is thrown.
    /// </summary>
    /// <param name="slug">The projects slug.</param>
    /// <exception cref="InvalidOperationException">Thrown when no project is found.</exception>
    /// <returns>The Id from the slug.</returns>
    public Task<int> GetProjectIdBySlug(string slug);

    /// <summary>
    /// Checks if the project slug is used.
    /// </summary>
    /// <param name="slug">The slug to check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the slug is used or not.</returns>
    public Task<bool> CheckProjectSlugExists(string slug);
}

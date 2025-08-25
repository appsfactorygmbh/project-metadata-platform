using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for all projects or one specific project with an id.
/// </summary>
public interface IProjectsRepository
{
    /// <summary>
    /// Returns a collection of all projects with specific search pattern.
    /// </summary>
    /// <param name="query">The query containing filters and search pattern.</param>
    /// <returns>An Enumeration of projects.</returns>
    Task<IEnumerable<Project>> GetProjectsAsync(GetAllProjectsQuery query);

    /// <summary>
    /// Returns a collection of all projects.
    /// </summary>
    /// <returns>An Enumeration of projects.</returns>
    Task<IEnumerable<Project>> GetProjectsAsync();

    /// <summary>
    /// Returns a project.
    /// </summary>
    /// <returns>One project or null.</returns>
    Task<Project> GetProjectAsync(int id);

    /// <summary>
    /// Returns a project.
    /// </summary>
    /// <returns>One project or null.</returns>
    /// <exception cref="ProjectNotFoundException">Thrown when the project is not found.</exception>
    Task<Project> GetProjectWithPluginsAsync(int id);

    /// <summary>
    /// Saves a project to the database.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    Task AddProjectAsync(Project project);

    /// <summary>
    /// Checks if a project exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True, if the project with the given id exists</returns>
    Task<bool> CheckProjectExists(int id);

    /// <summary>
    /// Deletes a project from the database.
    /// </summary>
    /// <param name="project">The project to delete.</param>
    /// <returns>A task representing the asynchronous operation, which upon completion returns the deleted project.</returns>
    Task<Project> DeleteProjectAsync(Project project);

    /// <summary>
    /// Returns the id of a project based on its unique slug.
    /// </summary>
    /// <param name="slug">The projects unique slug.</param>
    /// <returns>A task representing the asynchronous operation, which upon completion returns the requested project id or null.</returns>
    /// <exception cref="ProjectNotFoundException">Thrown when the project is not found.</exception>
    Task<int> GetProjectIdBySlugAsync(string slug);
}

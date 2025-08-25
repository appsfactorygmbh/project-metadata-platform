using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

namespace ProjectMetadataPlatform.Application.Helper;

/// <inheritdoc/>
public partial class SlugHelper : ISlugHelper
{
    [GeneratedRegex("[^a-zA-Z0-9_ ]")]
    private static partial Regex ProjectNameToSlugRegex();

    private readonly IProjectsRepository _projectsRepository;

    /// <summary>
    /// Creates a new instance of <see cref="SlugHelper" />.
    /// </summary>
    /// <param name="projectsRepository">Repository for Projects</param>
    public SlugHelper(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    /// <inheritdoc/>
    public string GenerateSlug(string input)
    {
        return ProjectNameToSlugRegex()
            .Replace(input, "")
            .Trim()
            .Replace(" ", "_")
            .ToLowerInvariant();
    }

    /// <inheritdoc/>
    public async Task<int> GetProjectIdBySlug(string slug)
    {
        return await _projectsRepository.GetProjectIdBySlugAsync(slug);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckProjectSlugExists(string slug)
    {
        try
        {
            _ = await _projectsRepository.GetProjectIdBySlugAsync(slug);
            return true;
        }
        catch (ProjectNotFoundException)
        {
            return false;
        }
    }
}

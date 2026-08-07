using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Service for managing Authorization Policies.
/// </summary>
public interface IAuthorizationAdminService
{
    /// <summary>
    /// Gets a List of all Resource Kinds that a policy exists for.
    /// </summary>
    /// <returns>List of Resource kinds</returns>
    Task<IEnumerable<string>> GetResources();
}

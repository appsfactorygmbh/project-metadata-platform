using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Service for Checking Authorzation Requests and Planning Resource Queries.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Marks Authorization as Checked for Internal Requests or Requests that dont need Authorization.
    /// </summary>
    /// <returns></returns>
    Task BypassAuthorization();

    /// <summary>
    /// Checks Access Request.
    /// </summary>
    /// <typeparam name="T">Type of Resource.</typeparam>
    /// <param name="resource">Requested Resource.</param>
    /// <param name="action">Requested Action.</param>
    /// <param name="updates">Optional Update Requests.</param>
    /// <returns>Boolean indicating if the access is allowd. </returns>
    Task<bool> CheckAccess<T>(
        T resource,
        AuthorizationConstants.Actions action,
        Dictionary<string, object?>? updates = null
    )
        where T : class;

    /// <summary>
    /// Creates a Query for getting accessible resources.
    /// </summary>
    /// <typeparam name="T">Type of Resource.</typeparam>
    /// <param name="query">Base Query.</param>
    /// <param name="attributeMap">Optional Dictionary for Mapping C# Attribute names to names used in policies.</param>
    /// <returns>Query containing authorized resources. Returns Null if Conversion Failed.</returns>
    Task<IQueryable<T>?> TryGetPlanResourceQuery<T>(
        IQueryable<T> query,
        Dictionary<string, string>? attributeMap = null
    )
        where T : class;

    /// <summary>
    /// Gets all allowed (not denied) actions for a Resource or its type.
    /// </summary>
    /// <typeparam name="T">Type of resource to check permissions on.</typeparam>
    /// <param name="resource"> Optional actual resource to check permissions on.</param>
    /// <returns>List of allowed action.</returns>
    Task<IEnumerable<AuthorizationConstants.Actions>> GetPermissions<T>(T? resource = null)
        where T : class;
}

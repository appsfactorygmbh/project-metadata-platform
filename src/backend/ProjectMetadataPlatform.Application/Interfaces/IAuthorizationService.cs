using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;

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
    /// <param name="token">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <param name="updates">Optional Updates Requests.</param>
    /// <returns>Dictionary with Access Results per Action </returns>
    Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        ApiToken token,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    );

    /// <summary>
    /// Checks Access Request.
    /// </summary>
    /// <param name="businessUnit">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <param name="updates">Optional Updates Requests.</param>
    /// <returns>Dictionary with Access Results per Action </returns>
    Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        BusinessUnit businessUnit,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    );

    /// <summary>
    /// Checks Access Request.
    /// </summary>
    /// <param name="department">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <param name="updates">Optional Updates Requests.</param>
    /// <returns>Dictionary with Access Results per Action </returns>
    Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Department department,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    );

    /// <summary>
    /// Checks Access Request.
    /// </summary>
    /// <param name="company">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <param name="updates">Optional Updates Requests.</param>
    /// <returns>Dictionary with Access Results per Action </returns>
    Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Company company,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    );

    /// <summary>
    /// Checks Access Request.
    /// </summary>
    /// <param name="location">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <param name="updates">Optional Updates Requests.</param>
    /// <returns>Dictionary with Access Results per Action </returns>
    Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        OfficeLocation location,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    );

    /// <summary>
    /// Checks Access Request.
    /// </summary>
    /// <param name="plugin">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <param name="updates">Optional Updates Requests.</param>
    /// <returns>Dictionary with Access Results per Action </returns>
    Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Plugin plugin,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    );

    /// <summary>
    /// Checks Access Request.
    /// </summary>
    /// <param name="project">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <param name="updates">Optional Updates Requests.</param>
    /// <returns>Dictionary with Access Results per Action </returns>
    Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Project project,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    );

    /// <summary>
    /// Checks Access Request.
    /// </summary>
    /// <param name="team">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <param name="updates">Optional Updates Requests.</param>
    /// <returns>Dictionary with Access Results per Action </returns>
    Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Team team,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    );

    /// <summary>
    /// Checks Access Request.
    /// </summary>
    /// <param name="user">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <param name="updates">Optional Updates Requests.</param>
    /// <returns>Dictionary with Access Results per Action </returns>
    Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        ApplicationUser user,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    );

    /// <summary>
    /// Checks Access Request.
    /// </summary>
    /// <param name="log">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <param name="updates">Optional Updates Requests.</param>
    /// <returns>Dictionary with Access Results per Action </returns>
    Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Log log,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    );

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
    );
}

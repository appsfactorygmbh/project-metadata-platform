using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Cerbos.Sdk;
using Cerbos.Sdk.Builder;
using Cerbos.Sdk.Utility;
using Microsoft.AspNetCore.Http;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;
using Principal = Cerbos.Sdk.Builder.Principal;
using Resource = Cerbos.Sdk.Builder.Resource;

namespace ProjectMetadataPlatform.Infrastructure.Authorization;

/// <summary>
/// Implements <see cref="IAuthorizationService"/>
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private readonly ICerbosClient _cerbosClient;

    private readonly IAuthorizationTracker _tracker;

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUsersRepository _usersRepository;

    private readonly IApiTokenRepository _apiTokenRepository;

    /// <summary>
    /// Constructor for <see cref="AuthorizationService"/>
    /// </summary>
    /// <param name="cerbosClient">Cerbos Authorization Client.</param>
    /// <param name="tracker">Authorization Tracker.</param>
    /// <param name="httpContextAccessor">Http Context Accessor for getting Principles from Tokens.</param>
    /// <param name="usersRepository">Repo for User access.</param>
    /// <param name="apiTokenRepository">Repo for ApiToken access.</param>
    public AuthorizationService(
        ICerbosClient cerbosClient,
        IAuthorizationTracker tracker,
        IHttpContextAccessor httpContextAccessor,
        IUsersRepository usersRepository,
        IApiTokenRepository apiTokenRepository
    )
    {
        _cerbosClient = cerbosClient;
        _tracker = tracker;
        _httpContextAccessor = httpContextAccessor;
        _usersRepository = usersRepository;
        _apiTokenRepository = apiTokenRepository;

        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        options.Converters.Add(new JsonStringEnumConverter());
        AuthorizationConverter.Options = options;
    }

    /// <inheritdoc/>
    public async Task BypassAuthorization()
    {
        _tracker.MarkAsChecked();
    }

    /// <inheritdoc/>
    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        ApiToken token,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    )
    {
        var result = (
            await CheckRequest(
                await GetPrincipalFromContext(),
                token.ToResource(nameof(ApiToken), token.Id.ToString(), updates),
                actions.Select(action => action.ToString())
            )
        ).Find(token.Id.ToString());
        return HandleCheckAuthorizationResult(actions, result);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        BusinessUnit businessUnit,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    )
    {
        var result = (
            await CheckRequest(
                await GetPrincipalFromContext(),
                businessUnit.ToResource(nameof(BusinessUnit), businessUnit.Id.ToString(), updates),
                actions.Select(action => action.ToString())
            )
        ).Find(businessUnit.Id.ToString());

        return HandleCheckAuthorizationResult(actions, result);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Company company,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    )
    {
        var result = (
            await CheckRequest(
                await GetPrincipalFromContext(),
                company.ToResource(nameof(Company), company.Id.ToString(), updates),
                actions.Select(action => action.ToString())
            )
        ).Find(company.Id.ToString());

        return HandleCheckAuthorizationResult(actions, result);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Department department,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    )
    {
        var result = (
            await CheckRequest(
                await GetPrincipalFromContext(),
                department.ToResource(nameof(Department), department.Id.ToString(), updates),
                actions.Select(action => action.ToString())
            )
        ).Find(department.Id.ToString());

        return HandleCheckAuthorizationResult(actions, result);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        OfficeLocation location,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    )
    {
        var result = (
            await CheckRequest(
                await GetPrincipalFromContext(),
                location.ToResource(nameof(OfficeLocation), location.Id.ToString(), updates),
                actions.Select(action => action.ToString())
            )
        ).Find(location.Id.ToString());

        return HandleCheckAuthorizationResult(actions, result);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Plugin plugin,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    )
    {
        var result = (
            await CheckRequest(
                await GetPrincipalFromContext(),
                plugin.ToResource(nameof(Plugin), plugin.Id.ToString(), updates),
                actions.Select(action => action.ToString())
            )
        ).Find(plugin.Id.ToString());

        return HandleCheckAuthorizationResult(actions, result);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Project project,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    )
    {
        var result = (
            await CheckRequest(
                await GetPrincipalFromContext(),
                project.ToResource(nameof(Project), project.Id.ToString(), updates),
                actions.Select(action => action.ToString())
            )
        ).Find(project.Id.ToString());

        return HandleCheckAuthorizationResult(actions, result);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Team team,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    )
    {
        var result = (
            await CheckRequest(
                await GetPrincipalFromContext(),
                team.ToResource(nameof(Team), team.Id.ToString(), updates),
                actions.Select(action => action.ToString())
            )
        ).Find(team.Id.ToString());

        return HandleCheckAuthorizationResult(actions, result);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        ApplicationUser user,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    )
    {
        var result = (
            await CheckRequest(
                await GetPrincipalFromContext(),
                user.ToResource(nameof(ApplicationUser), user.Id.ToString(), updates),
                actions.Select(action => action.ToString())
            )
        ).Find(user.Id.ToString());

        return HandleCheckAuthorizationResult(actions, result);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Log log,
        IEnumerable<AuthorizationConstants.Actions> actions,
        Dictionary<string, object?>? updates = null
    )
    {
        var result = (
            await CheckRequest(
                await GetPrincipalFromContext(),
                log.ToResource(nameof(Log), log.Id.ToString(), updates),
                actions.Select(action => action.ToString())
            )
        ).Find(log.Id.ToString());

        return HandleCheckAuthorizationResult(actions, result);
    }

    /// <inheritdoc/>
    public async Task<IQueryable<T>?> TryGetPlanResourceQuery<T>(
        IQueryable<T> query,
        Dictionary<string, string>? attributeMap = null
    )
    {
        try
        {
            var result = await PlanRequest(
                await GetPrincipalFromContext(),
                Resource
                    .NewInstance(typeof(T).Name)
                    .WithPolicyVersion(AuthorizationConstants.POLICY_VERSION),
                [AuthorizationConstants.Actions.GET.ToString()]
            );

            var authQuery = AuthorizationConverter.ConvertAstToQueryable(
                query,
                result.Filter,
                attributeMap
            );
            _tracker.MarkAsChecked();
            return authQuery;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{nameof(this.TryGetPlanResourceQuery)} failed: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets the Principal from its Authentication Method.
    /// </summary>
    /// <returns>Authorization Principle.</returns>
    /// <exception cref="UnknownAuthentificationMethodException">Thrown if Authentication Method is unknown.</exception>
    private async Task<Principal> GetPrincipalFromContext()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(
            ClaimTypes.AuthenticationMethod
        ) switch
        {
            "JWT Token" => await GetUserPrincipal(),
            "API Token" => await GetApiTokenPrincipalAsync(),
            _ => throw new UnknownAuthentificationMethodException(),
        };
    }

    /// <summary>
    /// Returns the User Principle from HttpContext.
    /// </summary>
    /// <returns>Authorization Principle.</returns>
    private async Task<Principal> GetUserPrincipal()
    {
        var email =
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
            ?? "Unknown user";
        var user = await _usersRepository.GetUserByEmailNoTrackingAsync(email);

        return user.ToPrincipal(AuthorizationConstants.PRINCIPLE_USER);
    }

    /// <summary>
    /// Returns the Token Principle from HttpContext.
    /// </summary>
    /// <returns>Authorization Principle.</returns>
    private async Task<Principal> GetApiTokenPrincipalAsync()
    {
        var name =
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)
            ?? "Unknown Token";
        var token = await _apiTokenRepository.GetApiTokenByName(name);

        return token.ToPrincipal(AuthorizationConstants.PRINCIPLE_TOKEN);
    }

    /// <summary>
    /// Execute a Authorization Access Request.
    /// </summary>
    /// <param name="principal">Authorization Principal.</param>
    /// <param name="resource">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <returns>Check Resources Response.</returns>
    private async Task<Cerbos.Sdk.Response.CheckResourcesResponse> CheckRequest(
        Principal principal,
        Resource resource,
        IEnumerable<string> actions
    )
    {
        var request = CheckResourcesRequest
            .NewInstance()
            .WithRequestId(RequestId.Generate())
            .WithPrincipal(principal)
            .WithResourceEntries(ResourceEntry.NewInstance(resource, [.. actions]))
            .WithIncludeMeta(true);
        var result = await _cerbosClient.CheckResourcesAsync(request);

        return result;
    }

    /// <summary>
    /// Execute a Authorization Plan Access Request.
    /// </summary>
    /// <param name="principal">Authorization Principal.</param>
    /// <param name="resource">Requested Resource.</param>
    /// <param name="actions">Requested Actions.</param>
    /// <returns>Plan Resources Response.</returns>
    private async Task<Cerbos.Sdk.Response.PlanResourcesResponse> PlanRequest(
        Principal principal,
        Resource resource,
        IEnumerable<string> actions
    )
    {
        var request = PlanResourcesRequest
            .NewInstance()
            .WithRequestId(RequestId.Generate())
            .WithPrincipal(principal)
            .WithResource(resource)
            .WithActions([.. actions]);
        var result = await _cerbosClient.PlanResourcesAsync(request);

        return result;
    }

    /// <summary>
    /// Converts a Check Authorization Result to a Dictionary of Actions and results.
    /// </summary>
    /// <param name="actions">Actions that where checked.</param>
    /// <param name="result">Authorization Result.</param>
    /// <returns>Result Dictionary.</returns>
    private Dictionary<AuthorizationConstants.Actions, bool> HandleCheckAuthorizationResult(
        IEnumerable<AuthorizationConstants.Actions> actions,
        Cerbos.Sdk.Response.CheckResourcesResponse.Types.ResultEntry result
    )
    {
        var accessDict = new Dictionary<AuthorizationConstants.Actions, bool> { };
        foreach (var action in actions)
        {
            accessDict.Add(action, result.IsAllowed(action.ToString()));
        }
        _tracker.MarkAsChecked();

        return accessDict;
    }
}

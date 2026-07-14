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
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
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
    public async Task<bool> CheckAccess<T>(
        T resource,
        AuthorizationConstants.Actions action,
        Dictionary<string, object?>? updates = null
    )
        where T : class
    {
        var result = (
            await CheckRequest(
                await GetPrincipalFromContext(),
                resource.ToResource(typeof(T).Name, "Default", updates),
                [action.ToString()]
            )
        ).Find("Default");
        _tracker.MarkAsChecked();
        return result.IsAllowed(action.ToString());
    }

    /// <inheritdoc/>
    public async Task<IQueryable<T>?> TryGetPlanResourceQuery<T>(
        IQueryable<T> query,
        Dictionary<string, string>? attributeMap = null
    )
        where T : class
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

    /// <inheritdoc/>
    public async Task<IEnumerable<AuthorizationConstants.Actions>> GetPermissions<T>(
        T? resource = null
    )
        where T : class
    {
        List<AuthorizationConstants.Actions> approvedActions = [];

        foreach (var action in Enum.GetValues<AuthorizationConstants.Actions>())
        {
            var authorizationResult = await PlanRequest(
                await GetPrincipalFromContext(),
                resource.ToResource(typeof(T).Name, "Default"),
                [action.ToString()]
            );
            if (
                authorizationResult.Filter.Kind
                != Cerbos.Api.V1.Engine.PlanResourcesFilter.Types.Kind.AlwaysDenied
            )
            {
                approvedActions.Add(action);
            }
        }
        return approvedActions;
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
}

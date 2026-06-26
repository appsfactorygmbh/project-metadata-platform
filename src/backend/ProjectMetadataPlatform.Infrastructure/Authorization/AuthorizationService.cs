using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;
using Principal = Cerbos.Sdk.Builder.Principal;
using Resource = Cerbos.Sdk.Builder.Resource;

namespace ProjectMetadataPlatform.Infrastructure.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly ICerbosClient _cerbosClient;

    private readonly IAuthorizationTracker _tracker;

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUsersRepository _usersRepository;

    private readonly IApiTokenRepository _apiTokenRepository;

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
    }

    public async Task BypassAuthorization()
    {
        _tracker.MarkAsChecked();
    }

    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        ApiToken token,
        IEnumerable<AuthorizationConstants.Actions> actions,
        IEnumerable<ResourceUpdate>? updates = null
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

    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        BusinessUnit businessUnit,
        IEnumerable<AuthorizationConstants.Actions> actions,
        IEnumerable<ResourceUpdate>? updates = null
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

    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Company company,
        IEnumerable<AuthorizationConstants.Actions> actions,
        IEnumerable<ResourceUpdate>? updates = null
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

    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        OfficeLocation location,
        IEnumerable<AuthorizationConstants.Actions> actions,
        IEnumerable<ResourceUpdate>? updates = null
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

    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Plugin plugin,
        IEnumerable<AuthorizationConstants.Actions> actions,
        IEnumerable<ResourceUpdate>? updates = null
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

    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Project project,
        IEnumerable<AuthorizationConstants.Actions> actions,
        IEnumerable<ResourceUpdate>? updates = null
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

    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        Team team,
        IEnumerable<AuthorizationConstants.Actions> actions,
        IEnumerable<ResourceUpdate>? updates = null
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

    public async Task<Dictionary<AuthorizationConstants.Actions, bool>> CheckAccess(
        ApplicationUser user,
        IEnumerable<AuthorizationConstants.Actions> actions,
        IEnumerable<ResourceUpdate>? updates = null
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
                    .NewInstance(nameof(T))
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
            return query;
        }
    }

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

    private async Task<Principal> GetUserPrincipal()
    {
        var email =
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
            ?? "Unknown user";
        var user = await _usersRepository.GetUserByEmailAsync(email);

        return user.ToPrincipal(AuthorizationConstants.PRINCIPLE_USER);
    }

    private async Task<Principal> GetApiTokenPrincipalAsync()
    {
        var name =
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)
            ?? "Unknown Token";
        var token = await _apiTokenRepository.GetApiTokenByName(name);

        return token.ToPrincipal(AuthorizationConstants.PRINCIPLE_TOKEN);
    }

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
            .WithResourceEntries(ResourceEntry.NewInstance(resource, [.. actions]));
        var result = await _cerbosClient.CheckResourcesAsync(request);

        return result;
    }

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

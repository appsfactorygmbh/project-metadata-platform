using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cerbos.Sdk;
using Cerbos.Sdk.Request;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Infrastructure.Authorization;

/// <summary>
/// Implements <see cref="IAuthorizationAdminService"/>
/// </summary>
public class AuthorizationAdminService : IAuthorizationAdminService
{
    private readonly ICerbosAdminClient _cerbosClient;

    /// <summary>
    /// Creates Instance of <see cref="AuthorizationAdminService"/>
    /// </summary>
    public AuthorizationAdminService(ICerbosAdminClient cerbosClient)
    {
        _cerbosClient = cerbosClient;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<string>> GetResources()
    {
        var policyIds = (
            await _cerbosClient.ListPoliciesAsync(ListPoliciesRequest.NewInstance())
        ).PolicyIds;
        var policies = await _cerbosClient.GetPolicyAsync(
            GetPolicyRequest.NewInstance([.. policyIds])
        );

        return policies
            .Policies.Where(policy => policy.Kind == Cerbos.Api.V1.Policy.Kind.Resource)
            .Select(policy => policy.ResourcePolicy.Resource);
    }
}

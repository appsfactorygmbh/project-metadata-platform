using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Helper;

public class PaginationHelper : IPaginationHelper
{
    private readonly IAuthorizationService _authorizationService;

    public PaginationHelper(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task<(
        IEnumerable<TResource> Resources,
        TResource? LastResource
    )> PaginateWithAuthAsync<TResource>(IQueryable<TResource> query, int? limit)
        where TResource : class
    {
        var queriedResources = await _authorizationService.TryGetPlanResourceQuery(query);
        TResource? lastResource = null;
        if (queriedResources == null)
        {
            List<TResource> filteredResources = [];
            await foreach (var resource in query.AsAsyncEnumerable())
            {
                if (
                    await _authorizationService.CheckAccess(
                        resource,
                        AuthorizationConstants.Actions.GET
                    )
                )
                {
                    filteredResources.Add(resource);
                    if (
                        limit.HasValue
                        && limit.Value > 0
                        && filteredResources.Count == limit.Value + 1
                    )
                    {
                        break;
                    }
                }
            }
            if (limit.HasValue && limit.Value > 0 && filteredResources.Count > limit.Value)
            {
                filteredResources.RemoveAt(filteredResources.Count - 1);
                lastResource = filteredResources.Last();
            }

            return (filteredResources, lastResource);
        }

        var paginatedResources =
            (limit.HasValue && limit.Value > 0)
                ? await queriedResources.Take(limit.Value + 1).ToListAsync()
                : await queriedResources.ToListAsync();
        if (limit.HasValue && limit.Value > 0 && paginatedResources.Count > limit.Value)
        {
            paginatedResources.RemoveAt(paginatedResources.Count - 1);
            lastResource = paginatedResources.Last();
        }

        return (paginatedResources, lastResource);
    }
}

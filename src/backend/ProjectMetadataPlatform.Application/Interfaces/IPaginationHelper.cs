using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Helper for Paginating List Responses
/// </summary>
public interface IPaginationHelper
{
    /// <summary>
    /// Applies Authorization filter and limit on a query.
    /// </summary>
    /// <typeparam name="TResource">Type of queried Resource.</typeparam>
    /// <param name="query">List of resources.</param>
    /// <param name="limit">Limit of returned resources.</param>
    /// <returns>paginated and authorized List of resources.</returns>
    Task<(
        IEnumerable<TResource> Resources,
        TResource? LastResource
    )> PaginateWithAuthAsync<TResource>(IQueryable<TResource> query, int? limit)
        where TResource : class;
}

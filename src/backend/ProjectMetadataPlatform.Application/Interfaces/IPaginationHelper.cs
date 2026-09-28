using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IPaginationHelper
{
    Task<(
        IEnumerable<TResource> Resources,
        TResource? LastResource
    )> PaginateWithAuthAsync<TResource>(IQueryable<TResource> query, int? limit)
        where TResource : class;
}

using System.Linq;
using ProjectMetadataPlatform.Api.Authorization.Models;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Authorization.Helper;

/// <summary>
/// Helper Class for mapping Domain Objects to response types.
/// </summary>
public static class MappingHelper
{
    /// <summary>
    /// Creates a GetFilterResponse from a filter tree.
    /// </summary>
    /// <param name="filter">Filter tree to be converted.</param>
    /// <returns>GetFilterResponse representing the filter tree</returns>
    public static GetFilterResponse ToFilterResponse(this FilterTree filter)
    {
        return new GetFilterResponse(
            filter.NodeValue,
            filter.ChildNodes?.Select(child => child.ToFilterResponse())?.ToList()
        );
    }
}

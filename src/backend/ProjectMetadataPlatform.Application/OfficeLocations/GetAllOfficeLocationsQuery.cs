using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Query to return all Office Locations.
/// </summary>
public record GetAllOfficeLocationsQuery(OfficeLocationCursor? Cursor, int? Limit)
    : IRequest<(
        IEnumerable<OfficeLocation>,
        IEnumerable<AuthorizationConstants.Actions>,
        OfficeLocationCursor?
    )>;

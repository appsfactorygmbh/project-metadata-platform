using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Query for getting all bu's.
/// </summary>
public record GetAllBusinessUnitsQuery(BusinessUnitCursor? Cursor, int? Limit)
    : IRequest<(
        IEnumerable<BusinessUnit>,
        IEnumerable<AuthorizationConstants.Actions>,
        BusinessUnitCursor?
    )>;

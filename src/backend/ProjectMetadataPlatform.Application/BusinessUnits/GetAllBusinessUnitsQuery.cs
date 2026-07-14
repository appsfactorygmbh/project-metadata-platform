using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Query for getting all bu's.
/// </summary>
public record GetAllBusinessUnitsQuery
    : IRequest<(IEnumerable<BusinessUnit>, IEnumerable<AuthorizationConstants.Actions>)>;

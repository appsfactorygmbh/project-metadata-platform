using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Query for getting an Office Location by Id.
/// </summary>
/// <param name="Id">Id of the office location.</param>
public record GetOfficeLocationQuery(int Id)
    : IRequest<(OfficeLocation, IEnumerable<AuthorizationConstants.Actions>)>;

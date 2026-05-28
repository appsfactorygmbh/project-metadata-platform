using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Query to return all Office Locations.
/// </summary>
public record GetAllOfficeLocationsQuery : IRequest<IEnumerable<OfficeLocation>>;

using MediatR;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Command to update an office location.
/// </summary>
/// <param name="Id">Id of the location.</param>
/// <param name="OfficeLocationName">New name for the Office Location.</param>
public record UpdateOfficeLocationCommand(int Id, string? OfficeLocationName = null)
    : IRequest<OfficeLocation>;

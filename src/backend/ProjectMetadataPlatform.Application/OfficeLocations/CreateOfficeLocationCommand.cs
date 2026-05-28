using MediatR;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Command for creating a new office location.
/// </summary>
/// <param name="OfficeLocationName">Name of the new Office Location.</param>
public record CreateOfficeLocationCommand(string OfficeLocationName) : IRequest<int>;

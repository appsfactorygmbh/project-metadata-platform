using MediatR;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Command to delete an Office Location.
/// </summary>
/// <param name="Id">Id of the office Location.</param>
public record DeleteOfficeLocationCommand(int Id) : IRequest;

using MediatR;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Command to Delete a Business Unit.
/// </summary>
/// <param name="Id">Id of the BU</param>
public record DeleteBusinessUnitCommand(int Id) : IRequest;

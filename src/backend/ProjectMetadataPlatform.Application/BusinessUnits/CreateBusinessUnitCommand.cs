using MediatR;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Command for creating a new BU.
/// </summary>
/// <param name="BusinessUnitName">Name of the New BU.</param>
public record CreateBusinessUnitCommand(string BusinessUnitName) : IRequest<int>;

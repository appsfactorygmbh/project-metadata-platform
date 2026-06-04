using MediatR;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Command for updating a BU.
/// </summary>
/// <param name="Id">Id of the BU.</param>
/// <param name="BusinessUnitName">New Name of the BU.</param>
public record UpdateBusinessUnitCommand(int Id, string? BusinessUnitName = null)
    : IRequest<BusinessUnit>;

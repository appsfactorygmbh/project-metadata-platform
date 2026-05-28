using MediatR;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Query to get a Business Unit by its id.
/// </summary>
/// <param name="Id">Id of the Business Unit.</param>
public record GetBusinessUnitQuery(int Id) : IRequest<BusinessUnit>;

using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Query for all Teams linked to a BU.
/// </summary>
/// <param name="Id">Id of the BU.</param>
public record GetLinkedTeamsQuery(int Id) : IRequest<List<int>>;

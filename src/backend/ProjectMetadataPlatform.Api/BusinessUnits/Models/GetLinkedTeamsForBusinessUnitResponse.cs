using System.Collections.Generic;

namespace ProjectMetadataPlatform.Api.BusinessUnits.Models;

/// <summary>
/// Record representing a list of Team Ids linked to business unit.
/// </summary>
/// <param name="TeamIds">List of Ids.</param>
public record GetLinkedTeamsForBusinessUnitResponse(List<int> TeamIds);

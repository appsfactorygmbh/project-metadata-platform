using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.BusinessUnits.Models;

/// <summary>
/// Record representing a single returned business unit.
/// </summary>
/// <param name="Id">Id of the business unit.</param>
/// <param name="BusinessUnitName">Name of the business unit.</param>
/// <param name="Permissions">Permissions on the Resource.</param>
public record GetBusinessUnitResponse(
    int Id,
    string BusinessUnitName,
    List<AuthorizationConstants.Actions>? Permissions = null
);

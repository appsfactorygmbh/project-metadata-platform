namespace ProjectMetadataPlatform.Api.BusinessUnits.Models;

/// <summary>
/// Record representing a single returned business unit.
/// </summary>
/// <param name="Id">Id of the business unit.</param>
/// <param name="BusinessUnitName">Name of the business unit.</param>
public record GetBusinessUnitResponse(int Id, string BusinessUnitName);

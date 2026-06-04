namespace ProjectMetadataPlatform.Api.BusinessUnits.Models;

/// <summary>
/// Represents a Request for updating a business unit.
/// </summary>
/// <param name="BusinessUnitName">Updated Business Unit name.</param>
public record UpdateBusinessUnitRequest(string? BusinessUnitName = null);

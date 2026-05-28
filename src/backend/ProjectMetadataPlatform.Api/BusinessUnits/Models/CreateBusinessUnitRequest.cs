namespace ProjectMetadataPlatform.Api.BusinessUnits.Models;

/// <summary>
/// Record representing a request to create a Business Unit.
/// </summary>
/// <param name="BusinessUnitName">Name of the new bu.</param>
public record CreateBusinessUnitRequest(string BusinessUnitName);

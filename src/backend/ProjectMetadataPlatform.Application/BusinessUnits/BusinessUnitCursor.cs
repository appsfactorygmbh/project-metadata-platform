using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Cursor Record pointing towards a BusinesssUnit Resource
/// </summary>
/// <param name="BusinessUnitName">Name of the BusinessUnit</param>
public record BusinessUnitCursor(string BusinessUnitName) : Cursor;

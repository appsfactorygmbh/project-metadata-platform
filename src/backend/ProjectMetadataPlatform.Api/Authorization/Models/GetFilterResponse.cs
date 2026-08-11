using System.Collections.Generic;

namespace ProjectMetadataPlatform.Api.Authorization.Models;

/// <summary>
/// Represents the Filter (or nodes of it) of a Permission.
/// </summary>
/// <param name="Value">Value of the node (can be a variable, a constant or an operator).</param>
/// <param name="Children">List of Child nodes.</param>
public record GetFilterResponse(string Value, List<GetFilterResponse>? Children);

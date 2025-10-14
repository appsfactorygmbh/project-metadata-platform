using System.Collections;
using System.Collections.Generic;

namespace ProjectMetadataPlatform.Api.Authorization.Models;

/// <summary>
/// Response for getPolicy
/// </summary>
/// <param name="Rules">List of policy rules.</param>
public record PolicyResponse(IEnumerable<string> Rules);

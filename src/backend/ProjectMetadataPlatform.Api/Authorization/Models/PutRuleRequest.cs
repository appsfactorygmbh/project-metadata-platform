using System.Collections;
using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Authorization.Models;

/// <summary>
/// Request to create a new policy rule.
/// </summary>
/// <param name="PolicyRule">Policy Rule to be created.</param>
public record PutRuleRequest(PolicyRule PolicyRule);

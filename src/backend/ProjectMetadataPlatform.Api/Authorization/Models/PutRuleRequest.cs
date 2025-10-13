using System.Collections;
using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Authorization.Models;

public record PutRuleRequest(PolicyRule PolicyRule);
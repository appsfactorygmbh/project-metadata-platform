using System.Collections;
using System.Collections.Generic;

namespace ProjectMetadataPlatform.Api.Authorization.Models;

public record PolicyResponse(IEnumerable<string> Rules);

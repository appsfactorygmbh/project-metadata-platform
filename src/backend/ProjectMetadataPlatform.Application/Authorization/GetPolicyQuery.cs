using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Authorization;

public record GetPolicyQuery : IRequest<IEnumerable<string>>;

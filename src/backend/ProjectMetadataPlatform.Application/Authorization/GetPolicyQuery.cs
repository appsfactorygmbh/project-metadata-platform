using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Query to get the Policy.
/// </summary>
public record GetPolicyQuery : IRequest<IEnumerable<string>>;

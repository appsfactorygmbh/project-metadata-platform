using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Query for getting a List of Resource names.
/// </summary>
public record GetResourcesQuery() : IRequest<IEnumerable<string>>;

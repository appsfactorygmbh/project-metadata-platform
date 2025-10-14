using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Command for creating a new Authoriation Rule.
/// </summary>
/// <param name="PolicyRule">Policy Rule to be created.</param>
public record PutRuleCommand(PolicyRule PolicyRule) : IRequest<bool>;

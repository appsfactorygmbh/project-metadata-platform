using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Authorization;

public record PutRuleCommand(PolicyRule PolicyRule) : IRequest<bool>;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MediatR;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

public record GetLinkedTeamsQuery(int Id) : IRequest<List<int>>;

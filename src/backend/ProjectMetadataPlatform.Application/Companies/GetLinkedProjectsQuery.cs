using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MediatR;

namespace ProjectMetadataPlatform.Application.Companies;

public record GetLinkedProjectsQuery(int Id) : IRequest<List<string>>;

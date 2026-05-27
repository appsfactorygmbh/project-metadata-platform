using System.Collections;
using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

public record CreateBusinessUnitCommand(string BusinessUnitName) : IRequest<int>;

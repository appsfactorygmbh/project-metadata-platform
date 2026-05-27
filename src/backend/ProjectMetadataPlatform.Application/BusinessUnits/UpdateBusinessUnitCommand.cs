using System.Collections;
using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

public record UpdateBusinessUnitCommand(int Id, string? BusinessUnitName = null)
    : IRequest<BusinessUnit>;

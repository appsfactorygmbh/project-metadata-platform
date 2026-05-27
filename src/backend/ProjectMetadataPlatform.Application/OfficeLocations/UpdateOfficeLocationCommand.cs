using System.Collections;
using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

public record UpdateOfficeLocationCommand(int Id, string? OfficeLocationName = null)
    : IRequest<OfficeLocation>;

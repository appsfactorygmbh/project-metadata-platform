using System.Collections;
using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Companies;

public record UpdateCompanyCommand(int Id, string? CompanyName = null)
    : IRequest<Company>;

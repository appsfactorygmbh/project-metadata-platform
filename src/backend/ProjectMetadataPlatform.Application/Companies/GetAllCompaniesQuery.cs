using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Query to return all Companies.
/// </summary>
public record GetAllCompaniesQuery : IRequest<IEnumerable<Company>>;

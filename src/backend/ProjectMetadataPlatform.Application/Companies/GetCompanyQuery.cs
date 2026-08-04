using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Query to return a specified Company.
/// </summary>
/// <param name="Id">Id of the Company.</param>
public record GetCompanyQuery(int Id)
    : IRequest<(Company, IEnumerable<AuthorizationConstants.Actions>)>;

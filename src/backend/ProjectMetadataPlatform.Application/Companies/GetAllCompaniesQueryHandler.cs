using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Handler for the <see cref="GetAllCompaniesQuery" />.
/// </summary>
public class GetAllCompaniesQueryHandler
    : IRequestHandler<
        GetAllCompaniesQuery,
        (IEnumerable<Company>, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllCompaniesQueryHandler" />.
    /// </summary>
    public GetAllCompaniesQueryHandler(
        ICompanyRepository companyRepository,
        IAuthorizationService authorizationService
    )
    {
        _companyRepository = companyRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handler for Query to return all Companies.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Companies and allowed actions.</returns>
    public async Task<(IEnumerable<Company>, IEnumerable<AuthorizationConstants.Actions>)> Handle(
        GetAllCompaniesQuery request,
        CancellationToken cancellationToken
    )
    {
        var companies = await _companyRepository.GetCompaniesAsync();
        var queriedCompanies = await _authorizationService.TryGetPlanResourceQuery(companies);
        var permissions = await _authorizationService.GetPermissions<Company>(            actions: [AuthorizationConstants.Actions.CREATE]);
        if (queriedCompanies == null)
        {
            List<Company> filteredCompanies = [];
            foreach (var company in companies)
            {
                if (
                    await _authorizationService.CheckAccess(
                        company,
                        AuthorizationConstants.Actions.GET
                    )
                )
                {
                    filteredCompanies.Add(company);
                }
            }
            return (
                filteredCompanies.OrderBy(company => company.CompanyName.ToLowerInvariant()),
                permissions
            );
        }
        return (
            (await queriedCompanies.ToListAsync(cancellationToken: cancellationToken)).OrderBy(
                company => company.CompanyName.ToLowerInvariant()
            ),
            permissions
        );
    }
}

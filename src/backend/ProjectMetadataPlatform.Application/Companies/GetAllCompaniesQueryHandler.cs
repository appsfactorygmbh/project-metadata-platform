using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Handler for the <see cref="GetAllCompaniesQuery" />.
/// </summary>
public class GetAllCompaniesQueryHandler
    : IRequestHandler<GetAllCompaniesQuery, IEnumerable<Company>>
{
    private readonly ICompanyRepository _companyRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllCompaniesQueryHandler" />.
    /// </summary>
    public GetAllCompaniesQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    /// <summary>
    /// Handler for Query to return all Companies.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Companies.</returns>
    public async Task<IEnumerable<Company>> Handle(
        GetAllCompaniesQuery request,
        CancellationToken cancellationToken
    )
    {
        var companies = await _companyRepository.GetCompaniesAsync();
        return companies.OrderBy(company => company.CompanyName.ToLowerInvariant());
    }
}

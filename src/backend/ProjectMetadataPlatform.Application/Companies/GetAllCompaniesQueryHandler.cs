using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Companies;

public class GetAllCompaniesQueryHandler
    : IRequestHandler<GetAllCompaniesQuery, IEnumerable<Company>>
{
    private readonly ICompanyRepository _companyRepository;

    public GetAllCompaniesQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<IEnumerable<Company>> Handle(
        GetAllCompaniesQuery request,
        CancellationToken cancellationToken
    )
    {
        var companies = await _companyRepository.GetCompaniesAsync();
        return companies.OrderBy(company =>
            company.CompanyName.ToLowerInvariant()
        );
    }
}

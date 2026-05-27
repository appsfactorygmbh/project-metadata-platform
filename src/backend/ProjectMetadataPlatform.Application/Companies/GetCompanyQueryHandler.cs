using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Companies;

public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, Company>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Company> Handle(
        GetCompanyQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _companyRepository.GetCompanyAsync(request.Id);
    }
}

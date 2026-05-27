using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Companies;

public class GetLinkedProjectsQueryHandler : IRequestHandler<GetLinkedProjectsQuery, List<string>>
{
    private readonly ICompanyRepository _companyRepository;

    public GetLinkedProjectsQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<List<string>> Handle(
        GetLinkedProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        return
        [
            .. (
                (await _companyRepository.GetCompanyWithProjectsAsync(request.Id)).Projects ?? []
            ).Select(project => project.Slug),
        ];
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Handler for the <see cref="GetLinkedProjectsQuery" />.
/// </summary>
public class GetLinkedProjectsQueryHandler : IRequestHandler<GetLinkedProjectsQuery, List<string>>
{
    private readonly ICompanyRepository _companyRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetLinkedProjectsQueryHandler" />.
    /// </summary>
    public GetLinkedProjectsQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    /// <summary>
    /// Handles Query to get all Project Slugs linked to a Company
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Project Slugs</returns>
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

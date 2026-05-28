using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Handler for the <see cref="GetCompanyQuery" />.
/// </summary>
public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, Company>
{
    private readonly ICompanyRepository _companyRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetCompanyQueryHandler" />.
    /// </summary>
    public GetCompanyQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    /// <summary>
    /// Handles Query to return a single Company.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Company.</returns>
    public async Task<Company> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        return await _companyRepository.GetCompanyAsync(request.Id);
    }
}

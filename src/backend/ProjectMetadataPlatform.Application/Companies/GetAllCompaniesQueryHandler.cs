using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        (IEnumerable<Company>, IEnumerable<AuthorizationConstants.Actions>, CompanyCursor?)
    >
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPaginationHelper _paginationHelper;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllCompaniesQueryHandler" />.
    /// </summary>
    public GetAllCompaniesQueryHandler(
        ICompanyRepository companyRepository,
        IAuthorizationService authorizationService,
        IPaginationHelper paginationHelper
    )
    {
        _companyRepository = companyRepository;
        _authorizationService = authorizationService;
        _paginationHelper = paginationHelper;
    }

    /// <summary>
    /// Handler for Query to return all Companies.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Companies and allowed actions.</returns>
    public async Task<(
        IEnumerable<Company>,
        IEnumerable<AuthorizationConstants.Actions>,
        CompanyCursor?
    )> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await _companyRepository.GetCompaniesAsync(request.Cursor);
        var (paginatedCompanies, lastCompany) = await _paginationHelper.PaginateWithAuthAsync(
            companies,
            request.Limit
        );
        var permissions = await _authorizationService.GetAllowedActions<Company>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        var nextCursor = lastCompany == null ? null : new CompanyCursor(lastCompany.CompanyName);
        return (paginatedCompanies, permissions, nextCursor);
    }
}

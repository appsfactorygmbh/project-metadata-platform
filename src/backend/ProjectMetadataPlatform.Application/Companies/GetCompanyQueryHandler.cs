using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Handler for the <see cref="GetCompanyQuery" />.
/// </summary>
public class GetCompanyQueryHandler
    : IRequestHandler<GetCompanyQuery, (Company, IEnumerable<AuthorizationConstants.Actions>)>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetCompanyQueryHandler" />.
    /// </summary>
    public GetCompanyQueryHandler(
        ICompanyRepository companyRepository,
        IAuthorizationService authorizationService
    )
    {
        _companyRepository = companyRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles Query to return a single Company.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Company and allowed actions.</returns>
    public async Task<(Company, IEnumerable<AuthorizationConstants.Actions>)> Handle(
        GetCompanyQuery request,
        CancellationToken cancellationToken
    )
    {
        var company = await _companyRepository.GetCompanyAsync(request.Id);
        if (!await _authorizationService.CheckAccess(company, AuthorizationConstants.Actions.GET))
        {
            throw new UnauthorizedException();
        }
        var permissions = await _authorizationService.GetPermissions(company,[AuthorizationConstants.Actions.EDIT,AuthorizationConstants.Actions.DELETE]);
        return (company, permissions);
    }
}

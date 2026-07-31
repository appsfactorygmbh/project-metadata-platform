using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Handler for the <see cref="GetLinkedProjectsQuery" />.
/// </summary>
public class GetLinkedProjectsQueryHandler : IRequestHandler<GetLinkedProjectsQuery, List<string>>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetLinkedProjectsQueryHandler" />.
    /// </summary>
    public GetLinkedProjectsQueryHandler(
        ICompanyRepository companyRepository,
        IAuthorizationService authorizationService
    )
    {
        _companyRepository = companyRepository;
        _authorizationService = authorizationService;
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
        var company = await _companyRepository.GetCompanyWithProjectsAsync(request.Id);
        if (!await _authorizationService.CheckAccess(company, AuthorizationConstants.Actions.GET))
        {
            throw new UnauthorizedException();
        }
        return [.. (company.Projects ?? []).Select(project => project.Slug)];
    }
}

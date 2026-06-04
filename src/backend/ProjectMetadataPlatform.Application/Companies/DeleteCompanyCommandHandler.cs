using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Handler for the <see cref="DeleteCompanyCommand" />.
/// </summary>
public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of <see cref="DeleteCompanyCommandHandler" />.
    /// </summary>
    public DeleteCompanyCommandHandler(
        ICompanyRepository companyRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _companyRepository = companyRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the Command to delete a Company.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="CompanyStillLinkedToProjectsException">Thrown if the company is still linked to a project.</exception>
    public async Task Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetCompanyWithProjectsAsync(request.Id);
        if (company.Projects != null && company.Projects.Count > 0)
        {
            throw new CompanyStillLinkedToProjectsException(
                company,
                [.. company.Projects.Select(project => project.Id)]
            );
        }
        _ = await _companyRepository.DeleteCompanyAsync(company);
        await _logRepository.AddCompanyLogForCurrentActor(company, Action.REMOVED_COMPANY, []);
        await _unitOfWork.CompleteAsync();
    }
}

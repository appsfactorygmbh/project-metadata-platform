using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Handler for the <see cref="CreateCompanyCommand" />.
/// </summary>
public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, int>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of <see cref="CreateCompanyCommandHandler" />.
    /// </summary>
    public CreateCompanyCommandHandler(
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
    /// Handles Command to Create a new Company.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of the new company.</returns>
    /// <exception cref="CompanyNameAlreadyExistsException">Thrown if company with same name already exists.</exception>
    public async Task<int> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        if (await _companyRepository.CheckIfCompanyNameExistsAsync(request.CompanyName))
        {
            throw new CompanyNameAlreadyExistsException(request.CompanyName);
        }

        var company = new Company { CompanyName = request.CompanyName };
        await AddCompanyLog(company);
        await _companyRepository.AddCompanyAsync(company);
        await _unitOfWork.CompleteAsync();

        return company.Id;
    }

    /// <summary>
    /// Adds a new Log Entry for the Created Company.
    /// </summary>
    /// <param name="company">Newly Created Company.</param>
    /// <returns></returns>
    private async Task AddCompanyLog(Company company)
    {
        var logChanges = new List<LogChange>
        {
            new LogChange
            {
                Property = nameof(Company.CompanyName),
                OldValue = "",
                NewValue = company.CompanyName,
            },
        };

        await _logRepository.AddCompanyLogForCurrentActor(
            company,
            Action.ADDED_COMPANY,
            logChanges
        );
    }
}

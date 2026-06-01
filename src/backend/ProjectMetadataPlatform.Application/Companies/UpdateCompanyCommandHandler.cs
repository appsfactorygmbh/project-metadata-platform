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
/// Handler for the <see cref="UpdateCompanyCommand" />.
/// </summary>
public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Company>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of <see cref="UpdateCompanyCommandHandler" />.
    /// </summary>
    public UpdateCompanyCommandHandler(
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
    /// Handles Command to update a Company
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Updated Company</returns>
    /// <exception cref="CompanyNameAlreadyExistsException">New Name of the Company already exists.</exception>
    public async Task<Company> Handle(
        UpdateCompanyCommand request,
        CancellationToken cancellationToken
    )
    {
        var company = await _companyRepository.GetCompanyAsync(request.Id);
        var logChanges = new List<LogChange> { };
        if (request.CompanyName != null && request.CompanyName != company.CompanyName)
        {
            if (
                !string.Equals(
                    request.CompanyName,
                    company.CompanyName,
                    System.StringComparison.OrdinalIgnoreCase
                ) && await _companyRepository.CheckIfCompanyNameExistsAsync(request.CompanyName)
            )
            {
                throw new CompanyNameAlreadyExistsException(request.CompanyName);
            }

            logChanges.Add(
                new LogChange
                {
                    Property = nameof(Company.CompanyName),
                    OldValue = company.CompanyName,
                    NewValue = request.CompanyName,
                }
            );
            company.CompanyName = request.CompanyName;
        }
        if (logChanges.Count > 0)
        {
            var updatedCompany = await _companyRepository.UpdateCompanyAsync(company);
            await _logRepository.AddCompanyLogForCurrentActor(
                company: company,
                action: Action.UPDATED_COMPANY,
                logChanges
            );
            await _unitOfWork.CompleteAsync();
            return updatedCompany;
        }

        return company;
    }
}

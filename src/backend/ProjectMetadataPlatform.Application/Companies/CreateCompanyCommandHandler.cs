using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Companies;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, int>
{
    private readonly ICompanyRepository _departmentRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCommandHandler(
        ICompanyRepository departmentRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _departmentRepository = departmentRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(
        CreateCompanyCommand request,
        CancellationToken cancellationToken
    )
    {
        if (
            await _departmentRepository.CheckIfCompanyNameExistsAsync(request.CompanyName)
        )
        {
            throw new CompanyNameAlreadyExistsException(request.CompanyName);
        }

        var department = new Company { CompanyName = request.CompanyName };
        await AddCompanyLog(department);
        await _departmentRepository.AddCompanyAsync(department);
        await _unitOfWork.CompleteAsync();

        return department.Id;
    }

    private async Task AddCompanyLog(Company department)
    {
        var logChanges = new List<LogChange>
        {
            new LogChange
            {
                Property = nameof(Company.CompanyName),
                OldValue = "",
                NewValue = department.CompanyName,
            },
        };

        await _logRepository.AddCompanyLogForCurrentActor(
            department,
            Action.ADDED_COMPANY,
            logChanges
        );
    }
}

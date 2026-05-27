using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

public class CreateBusinessUnitCommandHandler : IRequestHandler<CreateBusinessUnitCommand, int>
{
    private readonly IBusinessUnitRepository _departmentRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBusinessUnitCommandHandler(
        IBusinessUnitRepository departmentRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _departmentRepository = departmentRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(
        CreateBusinessUnitCommand request,
        CancellationToken cancellationToken
    )
    {
        if (
            await _departmentRepository.CheckIfBusinessUnitNameExistsAsync(request.BusinessUnitName)
        )
        {
            throw new BusinessUnitNameAlreadyExistsException(request.BusinessUnitName);
        }

        var department = new BusinessUnit { BusinessUnitName = request.BusinessUnitName };
        await AddBusinessUnitLog(department);
        await _departmentRepository.AddBusinessUnitAsync(department);
        await _unitOfWork.CompleteAsync();

        return department.Id;
    }

    private async Task AddBusinessUnitLog(BusinessUnit department)
    {
        var logChanges = new List<LogChange>
        {
            new LogChange
            {
                Property = nameof(BusinessUnit.BusinessUnitName),
                OldValue = "",
                NewValue = department.BusinessUnitName,
            },
        };

        await _logRepository.AddBusinessUnitLogForCurrentActor(
            department,
            Action.ADDED_DEPARTMENT,
            logChanges
        );
    }
}

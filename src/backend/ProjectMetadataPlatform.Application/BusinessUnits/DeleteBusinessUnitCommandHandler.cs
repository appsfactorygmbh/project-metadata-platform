using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

public class DeleteBusinessUnitCommandHandler : IRequestHandler<DeleteBusinessUnitCommand>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBusinessUnitCommandHandler(
        IBusinessUnitRepository businessUnitRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _businessUnitRepository = businessUnitRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteBusinessUnitCommand request, CancellationToken cancellationToken)
    {
        var businessUnit = await _businessUnitRepository.GetBusinessUnitWithTeamsAsync(request.Id);
        if (businessUnit.Teams != null && businessUnit.Teams.Count > 0)
        {
            throw new BusinessUnitStillLinkedToTeamsException(
                businessUnit,
                [.. businessUnit.Teams.Select(team => team.Id)]
            );
        }
        _ = await _businessUnitRepository.DeleteBusinessUnitAsync(businessUnit);
        await _logRepository.AddBusinessUnitLogForCurrentActor(
            businessUnit,
            Action.REMOVED_BUSINESS_UNIT,
            []
        );
        await _unitOfWork.CompleteAsync();
    }
}

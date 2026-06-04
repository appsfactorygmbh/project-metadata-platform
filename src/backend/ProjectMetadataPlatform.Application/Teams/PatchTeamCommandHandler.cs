using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.TeamExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Teams;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Teams;

/// <summary>
/// Handles the PatchGlobalPluginCommand request.
/// </summary>
public class PatchTeamCommandHandler : IRequestHandler<PatchTeamCommand, Team>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchGlobalPluginCommandHandler"/> class.
    /// </summary>
    /// <param name="teamRepository">The team repository to use for team operations.</param>
    /// <param name="businessUnitRepository">The business unit repository.</param>
    /// <param name="logRepository">The log repository to use for logging operations.</param>
    /// <param name="unitOfWork">The unit of work to use for transactional operations.</param>
    public PatchTeamCommandHandler(
        ITeamRepository teamRepository,
        IBusinessUnitRepository businessUnitRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _teamRepository = teamRepository;
        _businessUnitRepository = businessUnitRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the PatchTeamCommand request.
    /// </summary>
    /// <param name="request">The PatchTeamCommand request to handle.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the team that was updated.</returns>
    /// <exception cref="PluginNameAlreadyExistsException">The team name already exists.</exception>
    public async Task<Team> Handle(PatchTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetTeamAsync(request.Id);
        var changesLogs = new List<LogChange>();
        if (request.TeamName != null && request.TeamName != team.TeamName)
        {
            if (
                !string.Equals(request.TeamName, team.TeamName, StringComparison.OrdinalIgnoreCase)
                && await _teamRepository.CheckIfTeamNameExistsAsync(request.TeamName)
            )
            {
                throw new TeamNameAlreadyExistsException(request.TeamName);
            }
            changesLogs.Add(
                new()
                {
                    Property = nameof(team.TeamName),
                    OldValue = team.TeamName,
                    NewValue = request.TeamName,
                }
            );
            team.TeamName = request.TeamName;
        }
        if (request.BusinessUnitId != null && request.BusinessUnitId != team.BusinessUnitId)
        {
            if (
                !await _businessUnitRepository.CheckIfBusinessUnitExistsAsync(
                    request.BusinessUnitId.Value
                )
            )
            {
                throw new BusinessUnitNotFoundException(request.BusinessUnitId.Value);
            }
            changesLogs.Add(
                new()
                {
                    Property = nameof(team.BusinessUnit),
                    OldValue = team.BusinessUnit!.BusinessUnitName,
                    NewValue = await _businessUnitRepository.RetrieveNameForIdAsync(
                        request.BusinessUnitId.Value
                    ),
                }
            );
            team.BusinessUnitId = request.BusinessUnitId.Value;
        }
        if (request.PTL != null && request.PTL != team.PTL)
        {
            changesLogs.Add(
                new()
                {
                    Property = nameof(team.PTL),
                    OldValue = team.PTL ?? "null",
                    NewValue = request.PTL,
                }
            );
            team.PTL = request.PTL;
        }
        if (changesLogs.Count > 0)
        {
            var patchedTeam = await _teamRepository.UpdateTeamAsync(team);
            await _logRepository.AddTeamLogForCurrentActor(
                team: team,
                action: Action.UPDATED_TEAM,
                changes: changesLogs
            );
            await _unitOfWork.CompleteAsync();
            return patchedTeam;
        }
        return team;
    }
}

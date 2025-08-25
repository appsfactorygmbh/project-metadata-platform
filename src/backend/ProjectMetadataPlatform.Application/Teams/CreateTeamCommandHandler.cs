using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.TeamExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Teams;

/// <summary>
/// Handler for the <see cref="CreateTeamCommand" />
/// </summary>
public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, int>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of<see cref="CreateTeamCommandHandler" />.
    /// </summary>
    /// <param name="teamRepository">The repository for managing teams.</param>
    /// <param name="logRepository">The repository for managing logs.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    public CreateTeamCommandHandler(
        ITeamRepository teamRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _teamRepository = teamRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new Team with the given attributes.
    /// </summary>
    /// <param name="request">The request that needs to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The response of the request.</returns>
    /// <exception cref="TeamNameAlreadyExistsException">The Team name already exists.</exception>
    public async Task<int> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        if (await _teamRepository.CheckIfTeamNameExistsAsync(request.TeamName))
        {
            throw new TeamNameAlreadyExistsException(request.TeamName);
        }

        var team = new Team
        {
            TeamName = request.TeamName,
            BusinessUnit = request.BusinessUnit,
            PTL = request.PTL,
        };

        await AddTeamPluginLog(team, request);
        await _teamRepository.AddTeamAsync(team);
        await _unitOfWork.CompleteAsync();

        return team.Id;
    }

    private async Task AddTeamPluginLog(Team team, CreateTeamCommand request)
    {
        var logChanges = new List<LogChange>
        {
            new()
            {
                Property = nameof(Team.TeamName),
                OldValue = "",
                NewValue = request.TeamName,
            },
            new()
            {
                Property = nameof(Team.BusinessUnit),
                OldValue = "",
                NewValue = request.BusinessUnit,
            },
        };

        if (!string.IsNullOrWhiteSpace(request.PTL))
        {
            logChanges.Add(
                new()
                {
                    Property = nameof(Team.PTL),
                    OldValue = "",
                    NewValue = request.PTL,
                }
            );
        }
        await _logRepository.AddTeamLogForCurrentUser(team, Action.ADDED_TEAM, logChanges);
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Handler for the <see cref="CreateUserCommand" />
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApplicationUser>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILogRepository _logRepository;

    private readonly ITeamRepository _teamRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of <see cref="CreateUserCommandHandler" />.
    /// </summary>
    /// <param name="usersRepository">Repository for accessing user data.</param>
    /// <param name="logRepository">Repository for logging data.</param>
    /// <param name="unitOfWork">Unit of work for managing transactions.</param>
    public CreateUserCommandHandler(
        IUsersRepository usersRepository,
        ILogRepository logRepository,
        ITeamRepository teamRepository,
        IUnitOfWork unitOfWork
    )
    {
        _usersRepository = usersRepository;
        _logRepository = logRepository;
        _teamRepository = teamRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new User with the given data.
    /// </summary>
    /// <param name="request">Request for user creation.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The ID of the created user.</returns>
    public async Task<ApplicationUser> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        if (request.Password != null)
        {
            await _usersRepository.CheckPasswordFormat(request.Password);
        }

        Collection<Team> teams = [];
        Collection<Team> teamSupport = [];

        foreach (var team in request.Teams ?? [])
        {
            var teamObject = await _teamRepository.GetTeamByNameAsync(team);

            teams.Add(teamObject);
        }

        foreach (var team in request.TeamSupport ?? [])
        {
            var teamObject = await _teamRepository.GetTeamByNameAsync(team);

            teamSupport.Add(teamObject);
        }

        // Uses Email as Username because: Username cant be empty + Username cant be duplicate.
        var user = new ApplicationUser
        {
            Id = request.Id,
            Email = request.Email,
            UserName = request.Email,
            IsActive = request.IsActive,
            IsScimProvisioned = request.IsScimProvisioned,
            BusinessUnits = request.BusinessUnits?.Any() == true ? request.BusinessUnits : null,
            Company = request.Company,
            Departments = request.Departments?.Any() == true ? request.Departments : null,
            Teams = teams.Any() ? teams : null,
            TeamSupport = teamSupport.Any() ? teamSupport : null,
            JobTitles = request.JobTitles?.Any() == true ? request.JobTitles : null,
        };
        await AddCreatedUserLog(user);
        _ = await _usersRepository.CreateUserAsync(user, request.Password);

        await _unitOfWork.CompleteAsync();
        return user;
    }

    private async Task AddCreatedUserLog(ApplicationUser user)
    {
        var changes = new List<LogChange>
        {
            new()
            {
                OldValue = "",
                NewValue = user.Email!,
                Property = nameof(ApplicationUser.Email),
            },
        };
        if (user.Teams != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = String.Join(", ", user.Teams!.Select(t => t.TeamName)),
                    Property = nameof(ApplicationUser.Teams),
                }
            );
        }
        if (user.TeamSupport != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = String.Join(", ", user.TeamSupport!.Select(t => t.TeamName)),
                    Property = nameof(ApplicationUser.TeamSupport),
                }
            );
        }
        if (user.IsActive != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = user.IsActive.ToString()!,
                    Property = nameof(ApplicationUser.IsActive),
                }
            );
        }
        if (user.IsScimProvisioned != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = user.IsScimProvisioned.ToString()!,
                    Property = nameof(ApplicationUser.IsScimProvisioned),
                }
            );
        }
        if (user.BusinessUnits != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = String.Join(", ", user.BusinessUnits),
                    Property = nameof(ApplicationUser.BusinessUnits),
                }
            );
        }
        if (user.JobTitles != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = String.Join(", ", user.JobTitles),
                    Property = nameof(ApplicationUser.JobTitles),
                }
            );
        }
        if (user.Departments != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = String.Join(", ", user.Departments),
                    Property = nameof(ApplicationUser.Departments),
                }
            );
        }
        if (user.Company != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = user.Company,
                    Property = nameof(ApplicationUser.Company),
                }
            );
        }
        //TODO: handling for scim change log
        //await _logRepository.AddUserLogForCurrentUser(user, Domain.Logs.Action.ADDED_USER, changes);
    }
}

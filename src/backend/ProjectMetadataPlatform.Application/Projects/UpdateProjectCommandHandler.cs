using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Errors.TeamExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handler for the <see cref="CreateProjectCommand"/>.
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, int>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="UpdateProjectCommand"/>.
    /// </summary>
    public UpdateProjectCommandHandler(
        IProjectsRepository projectsRepository,
        ITeamRepository teamRepository,
        ICompanyRepository companyRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService
    )
    {
        _projectsRepository = projectsRepository;
        _teamRepository = teamRepository;
        _companyRepository = companyRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the request to update a project.
    /// </summary>
    /// <param name="request">Request to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response to the request</returns>
    /// <exception cref="ProjectNotFoundException">Thrown when the project is not found.</exception>
    public async Task<int> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project =
            await _projectsRepository.GetProjectAsync(request.Id)
            ?? throw new ProjectNotFoundException(request.Id);
        await CheckAuthorization(project, request);

        await UpdateProjectProperties(request, project);

        await _unitOfWork.CompleteAsync();

        return project.Id;
    }

    /// <summary>
    /// Updates the properties of a project based on the specified request.
    /// Logs all changes made to the project's properties.
    /// </summary>
    /// <param name="request">The request containing the new project properties.</param>
    /// <param name="project">The existing project to be updated.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ProjectSlugAlreadyExistsException">Thrown when a project with the same slug already exists.</exception>
    private async Task UpdateProjectProperties(UpdateProjectCommand request, Project project)
    {
        var changes = new List<LogChange>();

        if (project.TeamId != request.TeamId)
        {
            // if team will be + team id does not exists -> throw Exception
            if (
                request.TeamId != null
                && !await _teamRepository.CheckIfTeamExistsAsync(request.TeamId.Value)
            )
            {
                throw new TeamNotFoundException(request.TeamId.Value);
            }
            var change = new LogChange
            {
                Property = "Team",
                OldValue = project.Team == null ? "null" : project.Team.TeamName,
                NewValue =
                    request.TeamId == null
                        ? string.Empty
                        : await _teamRepository.RetrieveNameForIdAsync(request.TeamId.Value),
            };
            changes.Add(change);
            project.TeamId = request.TeamId;
        }

        if (project.ProjectName != request.ProjectName)
        {
            var changeName = new LogChange
            {
                Property = nameof(Project.ProjectName),
                OldValue = project.ProjectName,
                NewValue = request.ProjectName,
            };
            changes.Add(changeName);
            project.ProjectName = request.ProjectName;
        }

        if (project.ClientName != request.ClientName)
        {
            var change = new LogChange
            {
                Property = nameof(Project.ClientName),
                OldValue = project.ClientName,
                NewValue = request.ClientName,
            };
            changes.Add(change);
            project.ClientName = request.ClientName;
        }

        if (project.CompanyId != request.CompanyId)
        {
            if (!await _companyRepository.CheckIfCompanyExistsAsync(request.CompanyId))
            {
                throw new CompanyNotFoundException(request.CompanyId);
            }
            var change = new LogChange
            {
                Property = nameof(project.Company),
                OldValue = project.Company!.CompanyName,
                NewValue = await _companyRepository.RetrieveNameForIdAsync(request.CompanyId),
            };
            changes.Add(change);
            project.CompanyId = request.CompanyId;
        }

        if (project.CompanyState != request.CompanyState)
        {
            var change = new LogChange
            {
                Property = nameof(Project.CompanyState),
                OldValue = project.CompanyState.ToString(),
                NewValue = request.CompanyState.ToString(),
            };
            changes.Add(change);
            project.CompanyState = request.CompanyState;
        }

        if (project.IsmsLevel != request.IsmsLevel)
        {
            var change = new LogChange
            {
                Property = nameof(Project.IsmsLevel),
                OldValue = project.IsmsLevel.ToString(),
                NewValue = request.IsmsLevel.ToString(),
            };
            changes.Add(change);
            project.IsmsLevel = request.IsmsLevel;
        }
        if (project.Notes != request.Notes)
        {
            var notesInfo = new StringInfo(request.Notes);
            if (notesInfo.LengthInTextElements > 500)
            {
                throw new ProjectNotesSizeException(request.Notes.Length);
            }
            var change = new LogChange
            {
                Property = nameof(Project.Notes),
                OldValue =
                    project.Notes == ""
                        ? "null"
                        : (
                            project.Notes.Length > 50 ? project.Notes[0..50] + "..." : project.Notes
                        ),
                NewValue = request.Notes == "" ? "null" : request.Notes,
            };
            changes.Add(change);
            project.Notes = request.Notes;
        }
        if (project.IsEoC != request.IsEoC)
        {
            var change = new LogChange
            {
                Property = nameof(Project.IsEoC),
                OldValue = project.IsEoC.ToString(),
                NewValue = request.IsEoC.ToString(),
            };
            changes.Add(change);
            project.IsEoC = request.IsEoC;
        }
        if (project.IsArchived != request.IsArchived)
        {
            var archivedChanges = new List<LogChange>();

            var change = new LogChange
            {
                Property = nameof(Project.IsArchived),
                OldValue = project.IsArchived.ToString(),
                NewValue = request.IsArchived.ToString(),
            };
            archivedChanges.Add(change);

            await _logRepository.AddProjectLogForCurrentActor(
                project,
                request.IsArchived ? Action.ARCHIVED_PROJECT : Action.UNARCHIVED_PROJECT,
                archivedChanges
            );
            project.IsArchived = request.IsArchived;
        }

        if (changes.Count > 0)
        {
            await _logRepository.AddProjectLogForCurrentActor(
                project,
                Action.UPDATED_PROJECT,
                changes
            );
        }
    }

    /// <summary>
    /// Checks Authorization for a Project and its update request.
    /// </summary>
    /// <param name="project">Requested Project.</param>
    /// <param name="request">Update request for the project.</param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException">Thrown if Update Request is unauthorized</exception>
    private async Task CheckAuthorization(Project project, UpdateProjectCommand request)
    {
        Dictionary<string, object?> updates = [];
        if (request.ProjectName != project.ProjectName)
        {
            updates.Add(nameof(Project.ProjectName), request.ProjectName);
        }
        if (request.ClientName != project.ClientName)
        {
            updates.Add(nameof(Project.ClientName), request.ClientName);
        }
        if (request.CompanyId != project.CompanyId)
        {
            updates.Add(
                nameof(Project.Company),
                await _companyRepository.GetCompanyAsync(request.CompanyId)
            );
        }
        if (request.CompanyState != project.CompanyState)
        {
            updates.Add(nameof(Project.CompanyState), request.CompanyState);
        }
        if (request.TeamId != project.TeamId)
        {
            updates.Add(
                nameof(Project.Team),
                request.TeamId.HasValue
                    ? await _teamRepository.GetTeamAsync(request.TeamId.Value)
                    : null
            );
        }
        if (request.IsmsLevel != project.IsmsLevel)
        {
            updates.Add(nameof(Project.IsmsLevel), request.IsmsLevel);
        }
        if (request.IsArchived != project.IsArchived)
        {
            updates.Add(nameof(Project.IsArchived), request.IsArchived);
        }
        if (request.IsEoC != project.IsEoC)
        {
            updates.Add(nameof(Project.IsEoC), request.IsEoC);
        }
        if (request.Notes != project.Notes)
        {
            updates.Add(nameof(Project.Notes), request.Notes);
        }
        if (
            !await _authorizationService.CheckAccess(
                project,
                AuthorizationConstants.Actions.EDIT,
                updates
            )
        )
        {
            throw new UnauthorizedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handler for the <see cref="CreateProjectCommand"/>.
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, int>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IPluginRepository _pluginRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISlugHelper _slugHelper;

    /// <summary>
    /// Creates a new instance of <see cref="UpdateProjectCommand"/>.
    /// </summary>
    public UpdateProjectCommandHandler(
        IProjectsRepository projectsRepository,
        IPluginRepository pluginRepository,
        ITeamRepository teamRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork,
        ISlugHelper slugHelper
    )
    {
        _projectsRepository = projectsRepository;
        _pluginRepository = pluginRepository;
        _teamRepository = teamRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _slugHelper = slugHelper;
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
            await _projectsRepository.GetProjectWithPluginsAsync(request.Id)
            ?? throw new ProjectNotFoundException(request.Id);

        var globalPluginsById = (await _pluginRepository.GetGlobalPluginsAsync()).ToDictionary(
            plugin => plugin.Id
        );

        await UpdateProjectProperties(request, project);

        var invalidPluginIds = request
            .Plugins.Select(plugin => plugin.PluginId)
            .Distinct()
            .Where(pluginId => !globalPluginsById.ContainsKey(pluginId))
            .ToList();

        if (invalidPluginIds.Count > 0)
        {
            throw new MultiplePluginsNotFoundException(invalidPluginIds);
        }

        var currentPlugins = new List<ProjectPlugins>(project.ProjectPlugins ?? []);

        var existingPlugins = currentPlugins
            .IntersectBy(request.Plugins.Select(GetProjectPluginKey), GetProjectPluginKey)
            .ToList();

        var newPlugins = request
            .Plugins.ExceptBy(currentPlugins.Select(GetProjectPluginKey), GetProjectPluginKey)
            .ToList();

        var removedPlugins = currentPlugins.Except(existingPlugins).ToList();

        await AddNewPluginLogs(newPlugins, project, globalPluginsById);
        await AddRemovedPluginLogs(removedPlugins, project, globalPluginsById);
        await AddUpdatedPluginLogs(existingPlugins, project, request);

        project.ProjectPlugins = (project.ProjectPlugins ?? Enumerable.Empty<ProjectPlugins>())
            .Except(removedPlugins)
            .Concat(newPlugins)
            .ToList();

        await _unitOfWork.CompleteAsync();

        return project.Id;
    }

    /// <summary>
    /// Extracts the key components from the given project plugin.
    /// </summary>
    /// <param name="projectPlugin">The project plugin containing the key components.</param>
    /// <returns>A tuple containing the project ID, plugin ID, and URL.</returns>
    private static (int ProjectId, int PluginId, string Url) GetProjectPluginKey(
        ProjectPlugins projectPlugin
    ) => (projectPlugin.ProjectId, projectPlugin.PluginId, projectPlugin.Url);

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

        if (project.OfferId != request.OfferId)
        {
            var change = new LogChange
            {
                Property = nameof(Project.OfferId),
                OldValue = project.OfferId,
                NewValue = request.OfferId,
            };
            changes.Add(change);
            project.OfferId = request.OfferId;
        }

        if (project.Company != request.Company)
        {
            var change = new LogChange
            {
                Property = nameof(Project.Company),
                OldValue = project.Company,
                NewValue = request.Company,
            };
            changes.Add(change);
            project.Company = request.Company;
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

            await _logRepository.AddProjectLogForCurrentUser(
                project,
                request.IsArchived ? Action.ARCHIVED_PROJECT : Action.UNARCHIVED_PROJECT,
                archivedChanges
            );
            project.IsArchived = request.IsArchived;
        }

        if (changes.Count > 0)
        {
            await _logRepository.AddProjectLogForCurrentUser(
                project,
                Action.UPDATED_PROJECT,
                changes
            );
        }
    }

    private async Task AddNewPluginLogs(
        List<ProjectPlugins> newPlugins,
        Project project,
        Dictionary<int, Plugin> globalPluginsById
    )
    {
        foreach (
            var addedPluginChanges in newPlugins.Select(newPlugin => new List<LogChange>()
            {
                new()
                {
                    Property = nameof(ProjectPlugins.Plugin),
                    OldValue = string.Empty,
                    NewValue = globalPluginsById[newPlugin.PluginId].PluginName,
                },
                new()
                {
                    Property = nameof(ProjectPlugins.DisplayName),
                    OldValue = string.Empty,
                    NewValue = newPlugin.DisplayName ?? string.Empty,
                },
                new()
                {
                    Property = nameof(ProjectPlugins.Url),
                    OldValue = string.Empty,
                    NewValue = newPlugin.Url,
                },
            })
        )
        {
            await _logRepository.AddProjectLogForCurrentUser(
                project,
                Action.ADDED_PROJECT_PLUGIN,
                addedPluginChanges
            );
        }
    }

    private async Task AddRemovedPluginLogs(
        List<ProjectPlugins> removedPlugins,
        Project project,
        Dictionary<int, Plugin> globalPluginsById
    )
    {
        foreach (
            var removedPluginChanges in removedPlugins.Select(removedPlugin => new List<LogChange>()
            {
                new()
                {
                    Property = nameof(ProjectPlugins.Plugin),
                    OldValue = globalPluginsById[removedPlugin.PluginId].PluginName,
                    NewValue = string.Empty,
                },
                new()
                {
                    Property = nameof(ProjectPlugins.DisplayName),
                    OldValue = removedPlugin.DisplayName ?? string.Empty,
                    NewValue = string.Empty,
                },
                new()
                {
                    Property = nameof(ProjectPlugins.Url),
                    OldValue = removedPlugin.Url,
                    NewValue = string.Empty,
                },
            })
        )
        {
            await _logRepository.AddProjectLogForCurrentUser(
                project,
                Action.REMOVED_PROJECT_PLUGIN,
                removedPluginChanges
            );
        }
    }

    private async Task AddUpdatedPluginLogs(
        List<ProjectPlugins> existingPlugins,
        Project project,
        UpdateProjectCommand request
    )
    {
        foreach (var existingPlugin in existingPlugins)
        {
            var updatedPluginChanges = new List<LogChange>();

            var requestPlugin = request.Plugins.First(plugin =>
                GetProjectPluginKey(plugin) == GetProjectPluginKey(existingPlugin)
            );

            if (existingPlugin.DisplayName != requestPlugin.DisplayName)
            {
                updatedPluginChanges.Add(
                    new LogChange
                    {
                        Property = nameof(ProjectPlugins.DisplayName),
                        OldValue = existingPlugin.DisplayName ?? string.Empty,
                        NewValue = requestPlugin.DisplayName ?? string.Empty,
                    }
                );
                existingPlugin.DisplayName = requestPlugin.DisplayName;
            }

            if (existingPlugin.Url != requestPlugin.Url)
            {
                updatedPluginChanges.Add(
                    new LogChange
                    {
                        Property = nameof(ProjectPlugins.Url),
                        OldValue = existingPlugin.Url,
                        NewValue = requestPlugin.Url,
                    }
                );
            }

            if (updatedPluginChanges.Count > 0)
            {
                await _logRepository.AddProjectLogForCurrentUser(
                    project,
                    Action.UPDATED_PROJECT_PLUGIN,
                    updatedPluginChanges
                );
            }

            existingPlugin.Url = requestPlugin.Url;
        }
    }
}

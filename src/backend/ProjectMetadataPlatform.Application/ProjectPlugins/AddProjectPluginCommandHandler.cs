using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.ProjectPlugins;

/// <summary>
/// Handler for the <see cref="AddProjectPluginCommand" />
/// </summary>
public class AddProjectPluginCommandHandler : IRequestHandler<AddProjectPluginCommand, int>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="AddProjectPluginCommandHandler"/>
    /// </summary>
    /// <param name="projectsRepository"></param>
    /// <param name="pluginRepository"></param>
    /// <param name="logRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="authorizationService"></param>
    public AddProjectPluginCommandHandler(
        IProjectsRepository projectsRepository,
        IPluginRepository pluginRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService
    )
    {
        _projectsRepository = projectsRepository;
        _pluginRepository = pluginRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the request to add a new plugin to a project.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    /// <exception cref="ProjectPluginAlreadyExistsException"></exception>
    public async Task<int> Handle(
        AddProjectPluginCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var project = await _projectsRepository.GetProjectAsync(request.ProjectId);
        var plugin = new ProjectPlugin
        {
            ProjectId = request.ProjectId,
            PluginId = request.PluginId,
            DisplayName = request.Name,
            Url = request.Url,
            Project = project,
            Plugin = await _pluginRepository.GetGlobalPluginAsNoTrackingAsync(request.PluginId),
        };

        if (!await _authorizationService.CheckAccess(plugin, AuthorizationConstants.Actions.CREATE))
        {
            throw new UnauthorizedException();
        }
        plugin.Plugin = null;
        plugin.Project = null;
        plugin.PluginBilling = null;
        if (
            await _pluginRepository.CheckProjectPluginExists(
                plugin.ProjectId,
                plugin.PluginId,
                plugin.Url
            )
        )
        {
            throw new ProjectPluginAlreadyExistsException(
                plugin.ProjectId,
                plugin.PluginId,
                plugin.Url
            );
        }

        await AddProjectPluginLog(project, plugin);
        _ = await _pluginRepository.StoreProjectPlugin(plugin);
        await _unitOfWork.CompleteAsync();
        return plugin.Id;
    }

    private async Task AddProjectPluginLog(Project project, ProjectPlugin plugin)
    {
        var pluginChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "",
                NewValue = plugin.Url,
                Property = nameof(ProjectPlugin.Url),
            },
        };
        if (plugin.DisplayName != null)
        {
            pluginChanges.Add(
                new LogChange
                {
                    OldValue = "",
                    NewValue = plugin.DisplayName,
                    Property = nameof(ProjectPlugin.DisplayName),
                }
            );
        }

        await _logRepository.AddProjectLogForCurrentActor(
            project,
            Action.ADDED_PROJECT_PLUGIN,
            pluginChanges
        );
    }
}

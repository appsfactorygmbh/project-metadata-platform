using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.ProjectPlugins;

/// <summary>
/// Handler for the <see cref="UpdateProjectPluginCommand" />
/// </summary>
public class UpdateProjectPluginCommandHandler
    : IRequestHandler<UpdateProjectPluginCommand, ProjectPlugin>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new Instance of <see cref="UpdateProjectPluginCommandHandler" />
    /// </summary>
    /// <param name="projectsRepository"></param>
    /// <param name="pluginRepository"></param>
    /// <param name="logRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="authorizationService"></param>
    public UpdateProjectPluginCommandHandler(
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
    /// Handles the Request to update a project plugin.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ProjectNotFoundException"></exception>
    /// <exception cref="ProjectPluginAlreadyExistsException"></exception>
    public async Task<ProjectPlugin> Handle(
        UpdateProjectPluginCommand request,
        CancellationToken cancellationToken = default
    )
    {
        if (!await _projectsRepository.CheckProjectExists(request.ProjectId))
        {
            throw new ProjectNotFoundException(request.ProjectId);
        }
        var plugin = await _pluginRepository.GetProjectPluginAsync(
            request.ProjectId,
            request.ProjectpluginId
        );
        await CheckAuthorization(plugin, request);
        if (
            request.Url != plugin.Url
            && await _pluginRepository.CheckProjectPluginExists(
                plugin.ProjectId,
                plugin.PluginId,
                request.Url
            )
        )
        {
            throw new ProjectPluginAlreadyExistsException(
                plugin.ProjectId,
                plugin.PluginId,
                request.Url
            );
        }
        var pluginChanges = new List<LogChange> { };

        if (request.Name != plugin.DisplayName)
        {
            pluginChanges.Add(
                new()
                {
                    Property = nameof(ProjectPlugin.DisplayName),
                    OldValue = plugin.DisplayName ?? "null",
                    NewValue = request.Name,
                }
            );
            plugin.DisplayName = request.Name;
        }
        if (request.Url != plugin.Url)
        {
            if (
                await _pluginRepository.CheckProjectPluginExists(
                    plugin.ProjectId,
                    plugin.PluginId,
                    request.Url
                )
            )
            {
                throw new ProjectPluginAlreadyExistsException(
                    plugin.ProjectId,
                    plugin.PluginId,
                    request.Url
                );
            }
            pluginChanges.Add(
                new()
                {
                    Property = nameof(ProjectPlugin.Url),
                    OldValue = plugin.Url,
                    NewValue = request.Url,
                }
            );
            plugin.Url = request.Url;
        }
        if (pluginChanges.Count > 0)
        {
            await _logRepository.AddProjectLogForCurrentActor(
                plugin.Project!,
                Action.UPDATED_PROJECT_PLUGIN,
                pluginChanges
            );
        }

        await _unitOfWork.CompleteAsync();
        return plugin;
    }

    /// <summary>
    /// Checks Authorization for a Project Plugin and its update request.
    /// </summary>
    /// <param name="plugin">Requested Plugin</param>
    /// <param name="request">Update Request for the Plugin</param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException">Thrown if Update Request is unauthorized</exception>
    private async Task CheckAuthorization(ProjectPlugin plugin, UpdateProjectPluginCommand request)
    {
        Dictionary<string, object?> updates = [];
        if (request.Name != plugin.DisplayName)
        {
            updates.Add(nameof(ProjectPlugin.DisplayName), request.Name);
        }
        if (request.Url != plugin.Url)
        {
            updates.Add(nameof(ProjectPlugin.Url), request.Url);
        }

        if (
            !await _authorizationService.CheckAccess(
                plugin,
                AuthorizationConstants.Actions.EDIT,
                updates
            )
        )
        {
            throw new UnauthorizedException();
        }
    }
}

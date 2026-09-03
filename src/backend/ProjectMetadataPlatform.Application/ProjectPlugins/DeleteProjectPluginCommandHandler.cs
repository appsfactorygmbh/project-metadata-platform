using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.ProjectPlugins;

/// <summary>
/// Handler for the <see cref="DeleteProjectPluginCommand" />
/// </summary>
public class DeleteProjectPluginCommandHandler : IRequestHandler<DeleteProjectPluginCommand>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="DeleteProjectPluginCommandHandler"/>
    /// </summary>
    /// <param name="projectsRepository"></param>
    /// <param name="pluginRepository"></param>
    /// <param name="logRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="authorizationService"></param>
    public DeleteProjectPluginCommandHandler(
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
    /// Handles request to delete  a project plugin.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ProjectNotFoundException"></exception>
    /// <exception cref="UnauthorizedException"></exception>
    public async Task Handle(
        DeleteProjectPluginCommand request,
        CancellationToken cancellationToken = default
    )
    {
        if (!await _projectsRepository.CheckProjectExists(request.ProjectId))
        {
            throw new ProjectNotFoundException(request.ProjectId);
        }
        var plugin = await _pluginRepository.GetProjectPluginAsync(
            request.ProjectId,
            request.ProjectPluginId
        );
        if (!await _authorizationService.CheckAccess(plugin, AuthorizationConstants.Actions.DELETE))
        {
            throw new UnauthorizedException();
        }
        var removedPluginChanges = new List<LogChange>()
        {
            new()
            {
                Property = nameof(ProjectPlugin.Plugin),
                OldValue = plugin.Plugin!.PluginName,
                NewValue = string.Empty,
            },
            new()
            {
                Property = nameof(ProjectPlugin.DisplayName),
                OldValue = plugin.DisplayName ?? string.Empty,
                NewValue = string.Empty,
            },
            new()
            {
                Property = nameof(ProjectPlugin.Url),
                OldValue = plugin.Url,
                NewValue = string.Empty,
            },
        };

        await _logRepository.AddProjectLogForCurrentActor(
            plugin.Project!,
            Action.REMOVED_PROJECT_PLUGIN,
            removedPluginChanges
        );

        _ = await _pluginRepository.DeleteProjectPlugin(plugin);
        await _unitOfWork.CompleteAsync();
    }
}

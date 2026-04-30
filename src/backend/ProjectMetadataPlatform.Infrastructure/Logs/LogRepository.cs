using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using static System.DateTimeOffset;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Infrastructure.Logs;

/// <summary>
///  Repository for creating and accessing logs.
/// </summary>
public class LogRepository : RepositoryBase<Log>, ILogRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUsersRepository _usersRepository;

    private readonly IApiTokenRepository _apiTokenRepository;

    // TODO keep in sync with Action enum and the LogConverter in the Api project
    private static readonly Dictionary<Action, string> ActionMessages = new()
    {
        { Action.ADDED_PROJECT, "created a new project with properties: ," },
        { Action.UPDATED_PROJECT, "updated project properties: set from to," },
        { Action.ARCHIVED_PROJECT, "archived project" },
        { Action.UNARCHIVED_PROJECT, "unarchived project" },
        { Action.ADDED_PROJECT_PLUGIN, "added a plugin to project with properties: = ," },
        { Action.UPDATED_PROJECT_PLUGIN, "updated a plugin in project: set from to , " },
        { Action.REMOVED_PROJECT_PLUGIN, "removed a plugin from project with properties: = ," },
        { Action.ADDED_USER, "added a new user with properties: = ," },
        { Action.UPDATED_USER, "updated user properties: set from to , " },
        { Action.REMOVED_USER, "removed user" },
        { Action.REMOVED_PROJECT, "removed project" },
        { Action.ADDED_GLOBAL_PLUGIN, "added a new global plugin with properties: = ," },
        { Action.UPDATED_GLOBAL_PLUGIN, "updated global plugin properties: set from to , " },
        { Action.ARCHIVED_GLOBAL_PLUGIN, "archived global plugin" },
        { Action.UNARCHIVED_GLOBAL_PLUGIN, "unarchived global plugin" },
        { Action.REMOVED_GLOBAL_PLUGIN, "removed global plugin" },
        { Action.ADDED_TEAM, "created a new team with properties: ," },
        { Action.UPDATED_TEAM, "updated team properties: set from to," },
        { Action.REMOVED_TEAM, "removed team" },
        { Action.ADDED_API_TOKEN, "created a new API token with properties: ," },
        { Action.REGENERATED_API_TOKEN, "regenerated the API token" },
        { Action.REMOVED_API_TOKEN, "removed the API token with properties: ," },
    };

    /// <summary>
    /// initialising context and httpContextAccessor to provide user information
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="usersRepository"></param>
    /// <param name="apiTokenRepository"></param>
    public LogRepository(
        ProjectMetadataPlatformDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        IUsersRepository usersRepository,
        IApiTokenRepository apiTokenRepository
    )
        : base(dbContext)
    {
        _context = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _usersRepository = usersRepository;
        _apiTokenRepository = apiTokenRepository;
    }

    ///  <inheritdoc />
    public async Task AddProjectLogForCurrentActor(
        Project project,
        Action action,
        List<LogChange> changes
    )
    {
        var actionWhiteList = new List<Action>
        {
            Action.ADDED_PROJECT,
            Action.ADDED_PROJECT_PLUGIN,
            Action.UPDATED_PROJECT,
            Action.UPDATED_PROJECT_PLUGIN,
            Action.REMOVED_PROJECT_PLUGIN,
            Action.ARCHIVED_PROJECT,
            Action.UNARCHIVED_PROJECT,
            Action.REMOVED_PROJECT,
        };

        if (!actionWhiteList.Contains(action))
        {
            throw new LogActionNotSupportedException(action, nameof(project));
        }

        var log = await PrepareGenericLogForCurrentActor(action, changes);

        log.Project = project;
        log.ProjectId = project.Id;
        log.ProjectName = _context
            .Entry(project)
            .Property<string>(nameof(Project.ProjectName))
            .OriginalValue;

        _ = _context.Logs.Add(log);
    }

    ///  <inheritdoc />
    public async Task AddUserLogForCurrentActor(
        ApplicationUser affectedUser,
        Action action,
        List<LogChange> changes
    )
    {
        var actionWhiteList = new List<Action>
        {
            Action.ADDED_USER,
            Action.UPDATED_USER,
            Action.REMOVED_USER,
        };

        if (!actionWhiteList.Contains(action))
        {
            throw new LogActionNotSupportedException(action, nameof(affectedUser));
        }

        var log = await PrepareGenericLogForCurrentActor(action, changes);

        log.AffectedUser = affectedUser;
        log.AffectedUserId = affectedUser.Id;
        log.AffectedUserEmail = _context
            .Entry(affectedUser)
            .Property<string?>(nameof(ApplicationUser.Email))
            .OriginalValue;

        _ = _context.Logs.Add(log);
    }

    ///  <inheritdoc />
    public async Task AddGlobalPluginLogForCurrentActor(
        Plugin globalPlugin,
        Action action,
        List<LogChange> changes
    )
    {
        var actionWhiteList = new List<Action>
        {
            Action.ADDED_GLOBAL_PLUGIN,
            Action.UPDATED_GLOBAL_PLUGIN,
            Action.ARCHIVED_GLOBAL_PLUGIN,
            Action.UNARCHIVED_GLOBAL_PLUGIN,
            Action.REMOVED_GLOBAL_PLUGIN,
        };

        if (!actionWhiteList.Contains(action))
        {
            throw new LogActionNotSupportedException(action, nameof(globalPlugin));
        }

        var log = await PrepareGenericLogForCurrentActor(action, changes);

        log.GlobalPlugin = globalPlugin;
        log.GlobalPluginId = globalPlugin.Id;
        log.GlobalPluginName = _context
            .Entry(globalPlugin)
            .Property<string>(nameof(Plugin.PluginName))
            .OriginalValue;

        _ = _context.Logs.Add(log);
    }

    ///  <inheritdoc />
    public async Task AddTeamLogForCurrentActor(Team team, Action action, List<LogChange> changes)
    {
        var actionWhiteList = new List<Action>
        {
            Action.ADDED_TEAM,
            Action.UPDATED_TEAM,
            Action.REMOVED_TEAM,
        };

        if (!actionWhiteList.Contains(action))
        {
            throw new LogActionNotSupportedException(action, nameof(team));
        }

        var log = await PrepareGenericLogForCurrentActor(action, changes);

        log.Team = team;
        log.TeamId = team.Id;
        log.TeamName = team.TeamName;
        _ = _context.Logs.Add(log);
    }

    ///  <inheritdoc />
    public async Task AddApiTokenLogForCurrentActor(
        ApiToken affectedToken,
        Action action,
        List<LogChange> changes
    )
    {
        var actionWhiteList = new List<Action>
        {
            Action.ADDED_API_TOKEN,
            Action.REGENERATED_API_TOKEN,
            Action.REMOVED_API_TOKEN,
        };

        if (!actionWhiteList.Contains(action))
        {
            throw new LogActionNotSupportedException(action, nameof(affectedToken));
        }

        var log = await PrepareGenericLogForCurrentActor(action, changes);

        log.AffectedToken = affectedToken;
        log.AffectedTokenId = affectedToken.Id;
        log.AffectedTokenName = affectedToken.Name;

        _ = _context.Logs.Add(log);
    }

    /// <summary>
    /// Prepares a generic log entry for the current user or token.
    /// </summary>
    /// <param name="action">The action performed by the user.</param>
    /// <param name="changes">The list of changes associated with the action.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the prepared log entry.</returns>
    private async Task<Log> PrepareGenericLogForCurrentActor(Action action, List<LogChange> changes)
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(
            ClaimTypes.AuthenticationMethod
        ) switch
        {
            "JWT Token" => await PrepareGenericLogForCurrentUser(action, changes),
            "API Token" => await PrepareGenericLogForCurrentApiToken(action, changes),
            _ => throw new UnknownAuthentificationMethodException(),
        };
    }

    /// <summary>
    /// Prepares a generic log entry for the current user.
    /// </summary>
    /// <param name="action">The action performed by the user.</param>
    /// <param name="changes">The list of changes associated with the action.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the prepared log entry.</returns>
    private async Task<Log> PrepareGenericLogForCurrentUser(Action action, List<LogChange> changes)
    {
        var email =
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
            ?? "Unknown user";
        var author = await _usersRepository.GetUserByEmailAsync(email);

        var log = new Log
        {
            AuthorName = author.Email,
            AuthorId = author.Id,
            AuthorTokenId = null,
            Action = action,
            TimeStamp = UtcNow,
            Changes = changes,
        };
        return log;
    }

    /// <summary>
    /// Prepares a generic log entry for the current token.
    /// </summary>
    /// <param name="action">The action performed by the user.</param>
    /// <param name="changes">The list of changes associated with the action.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the prepared log entry.</returns>
    private async Task<Log> PrepareGenericLogForCurrentApiToken(
        Action action,
        List<LogChange> changes
    )
    {
        var name =
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)
            ?? "Unknown Token";
        var author = await _apiTokenRepository.GetApiTokenByName(name);

        var log = new Log
        {
            AuthorName = author.Name,
            AuthorId = null,
            AuthorTokenId = author.Id,
            AuthorToken = author,
            Action = action,
            TimeStamp = UtcNow,
            Changes = changes,
        };
        return log;
    }

    ///  <inheritdoc />
    public async Task<List<Log>> GetLogsForProject(int projectId)
    {
        var res = _context
            .Logs.Include(l => l.Changes)
            .Include(l => l.Project)
            .Include(l => l.Author)
            .Include(l => l.AuthorToken)
            .Where(log => log.ProjectId == projectId);
        return SortByTimestamp(await res.ToListAsync());
    }

    ///  <inheritdoc />
    public async Task<List<Log>> GetLogsWithSearch(string search)
    {
        var lowerSearch = search.ToLower();

        var actionsToInclude = ActionMessages
            .Keys.Where(action => ActionMessages[action].Contains(lowerSearch))
            .ToList();

        var res = _context
            .Logs.Include(l => l.Changes)
            .Include(l => l.Project)
            .Include(l => l.Author)
            .Include(l => l.AuthorToken)
            .Include(l => l.AffectedUser)
            .Include(l => l.GlobalPlugin)
            .Where(log =>
                (
                    log.AuthorName != null
                    && EF.Functions.Like(log.AuthorName.ToLower(), $"%{lowerSearch}%")
                )
                || (
                    log.AffectedUserEmail != null
                    && EF.Functions.Like(log.AffectedUserEmail.ToLower(), $"%{lowerSearch}%")
                )
                || (
                    log.GlobalPluginName != null
                    && EF.Functions.Like(log.GlobalPluginName.ToLower(), $"%{lowerSearch}%")
                )
                || actionsToInclude.Contains(log.Action)
                || (
                    log.Project != null
                    && EF.Functions.Like(log.Project.ProjectName.ToLower(), $"%{lowerSearch}%")
                )
                || (
                    log.Changes != null
                    && log.Changes.Any(change =>
                        EF.Functions.Like(change.Property.ToLower(), $"%{lowerSearch}%")
                        || EF.Functions.Like(change.OldValue.ToLower(), $"%{lowerSearch}%")
                        || EF.Functions.Like(change.NewValue.ToLower(), $"%{lowerSearch}%")
                    )
                )
            );

        return SortByTimestamp(await res.ToListAsync());
    }

    ///  <inheritdoc />
    public async Task<List<Log>> GetLogsForUser(string userId)
    {
        var test = await _context.Logs.ToListAsync();
        var res = _context
            .Logs.Include(l => l.Changes)
            .Include(l => l.AffectedUser)
            .Include(l => l.AuthorToken)
            .Include(l => l.Author)
            .Where(log => log.AffectedUserId == userId);
        return SortByTimestamp(await res.ToListAsync());
    }

    ///  <inheritdoc />
    public async Task<List<Log>> GetLogsForGlobalPlugin(int globalPluginId)
    {
        var res = _context
            .Logs.Include(l => l.Changes)
            .Include(l => l.GlobalPlugin)
            .Include(l => l.Author)
            .Include(l => l.AuthorToken)
            .Where(log => log.GlobalPluginId == globalPluginId);
        return SortByTimestamp(await res.ToListAsync());
    }

    ///  <inheritdoc />
    public async Task<List<Log>> GetAllLogs()
    {
        return SortByTimestamp(
            await GetEverything()
                .Include(log => log.Project)
                .Include(log => log.Team)
                .Include(log => log.Author)
                .Include(l => l.AuthorToken)
                .Include(log => log.Changes)
                .ToListAsync()
        );
    }

    /// <summary>
    /// Sorts a list of logs by their timestamp.
    /// </summary>
    /// <param name="logs">The list of logs to be sorted.</param>
    /// <returns>A list of logs sorted by timestamp.</returns>
    private static List<Log> SortByTimestamp(List<Log> logs)
    {
        return [.. logs.OrderByDescending(log => log.TimeStamp)];
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Api.Logs.Models;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Users;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Api.Logs;

/// <inheritdoc />
public class LogConverter : ILogConverter
{
    // TODO keep in sync with Action enum and the LogRepository in the Infrastructure project

    /// <inheritdoc />
    public LogResponse BuildLogMessage(Log log)
    {
        var message = log switch
        {
            { Author.Email: not null } => GetNameFromEmail(log.Author.Email),
            { AuthorToken.Name: not null } => log.AuthorToken.Name,
            { AuthorName: not null } => GetNameFromEmail(log.AuthorName) + " (deleted actor)",
            _ => "<Deleted Actor>",
        };

        message +=
            " "
            + log.Action switch
            {
                Action.ADDED_PROJECT => BuildAddedProjectMessage(log.Changes),
                Action.UPDATED_PROJECT => BuildUpdatedProjectMessage(
                    log.Changes,
                    log.ProjectName ?? "<Unknown Project>"
                ),
                Action.ARCHIVED_PROJECT => BuildArchivedProjectMessage(log.ProjectName),
                Action.UNARCHIVED_PROJECT => BuildUnArchivedProjectMessage(log.ProjectName),
                Action.ADDED_PROJECT_PLUGIN => BuildAddedProjectPluginMessage(
                    log.ProjectName,
                    log.Changes
                ),
                Action.UPDATED_PROJECT_PLUGIN => BuildUpdatedProjectPluginMessage(
                    log.ProjectName,
                    log.Changes
                ),
                Action.REMOVED_PROJECT_PLUGIN => BuildRemovedProjectPluginMessage(
                    log.ProjectName,
                    log.Changes
                ),
                Action.ADDED_USER => BuildAddedUserMessage(log.Changes),
                Action.UPDATED_USER => BuildUpdatedUserMessage(log),
                Action.REMOVED_USER => BuildRemovedUserMessage(
                    log.AffectedUserEmail ?? "<Unknown User>"
                ),
                Action.REMOVED_PROJECT => BuildRemovedProjectMessage(
                    log.ProjectName ?? "<Unknown Project>"
                ),
                Action.ADDED_GLOBAL_PLUGIN => BuildAddedGlobalPluginMessage(log.Changes),
                Action.UPDATED_GLOBAL_PLUGIN => BuildUpdatedGlobalPluginMessage(
                    log.Changes,
                    log.GlobalPluginName ?? "<Unknown Plugin>"
                ),
                Action.ARCHIVED_GLOBAL_PLUGIN => BuildArchivedGlobalPluginMessage(
                    log.GlobalPluginName ?? "<Unknown Plugin>"
                ),
                Action.UNARCHIVED_GLOBAL_PLUGIN => BuildUnArchivedGlobalPluginMessage(
                    log.GlobalPluginName ?? "<Unknown Plugin>"
                ),
                Action.REMOVED_GLOBAL_PLUGIN => BuildRemovedGlobalPluginMessage(
                    log.GlobalPluginName ?? "<Unknown Plugin>"
                ),
                Action.ADDED_TEAM => BuildAddedTeamMessage(log.Changes),
                Action.UPDATED_TEAM => BuildUpdatedTeamMessage(
                    log.Changes,
                    log.TeamName ?? "<Unknown Team>"
                ),
                Action.REMOVED_TEAM => BuildRemovedTeamMessage(log.TeamName ?? "<Unknown Team>"),
                Action.ADDED_API_TOKEN => BuildAddedApiTokenMessage(log.Changes),
                Action.REMOVED_API_TOKEN => BuildRemovedApiTokenMessage(
                    log.Changes,
                    log.AffectedTokenName ?? "<Unknown Token>"
                ),
                Action.REGENERATED_API_TOKEN => BuildRegeneratedApiTokenMessage(
                    log.AffectedTokenName ?? "<Unknown Token>"
                ),
                Action.ADDED_OFFICE_LOCATION => BuildAddedOfficeLocationMessage(log.Changes),
                Action.UPDATED_OFFICE_LOCATION => BuildUpdatedOfficeLocationMessage(log),
                Action.REMOVED_OFFICE_LOCATION => BuildRemovedOfficeLocationMessage(
                    log.OfficeLocationName ?? "<Unknown Office Location>"
                ),
                Action.ADDED_BUSINESS_UNIT => BuildAddedBusinessUnitMessage(log.Changes),
                Action.UPDATED_BUSINESS_UNIT => BuildUpdatedBusinessUnitMessage(log),
                Action.REMOVED_BUSINESS_UNIT => BuildRemovedBusinessUnitMessage(
                    log.BusinessUnitName ?? "<Unknown Business Unit>"
                ),
                Action.ADDED_COMPANY => BuildAddedCompanyMessage(log.Changes),
                Action.UPDATED_COMPANY => BuildUpdatedCompanyMessage(log),
                Action.REMOVED_COMPANY => BuildRemovedCompanyMessage(
                    log.CompanyName ?? "<Unknown Company>"
                ),
                Action.ADDED_DEPARTMENT => BuildAddedDepartmentMessage(log.Changes),
                Action.UPDATED_DEPARTMENT => BuildUpdatedDepartmentMessage(log),
                Action.REMOVED_DEPARTMENT => BuildRemovedDepartmentMessage(
                    log.DepartmentName ?? "<Unknown Department>"
                ),
                _ => "",
            };

        return new LogResponse(message, GetTimestamp(log.TimeStamp));
    }

    /// <summary>
    /// Builds a message for an added project.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedTeamMessage(List<LogChange>? changes)
    {
        var message = "created a new team";
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.NewValue}")
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an updated team.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <param name="teamName">The name of the updated team.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedTeamMessage(List<LogChange>? changes, string teamName)
    {
        var message = $"updated team {teamName}: ";
        if (changes == null)
        {
            return message;
        }
        message += string.Join(
            ", ",
            changes.Select(change =>
                $" set {change.Property} from {change.OldValue} to {change.NewValue}"
            )
        );
        return message;
    }

    /// <summary>
    /// Builds a message for a removed team.
    /// </summary>
    /// <param name="teamName">The name of the removed team.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedTeamMessage(string teamName)
    {
        return "removed team " + teamName;
    }

    /// <summary>
    /// Builds a message for an added project.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedProjectMessage(List<LogChange>? changes)
    {
        var message = "created a new project";
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.NewValue}")
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an updated project.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <param name="projectName">The name of the updated project.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedProjectMessage(List<LogChange>? changes, string projectName)
    {
        var message = $"updated project {projectName}: ";
        if (changes == null)
        {
            return message;
        }
        message += string.Join(
            ", ",
            changes.Select(change =>
                $" set {change.Property} from {change.OldValue} to {change.NewValue}"
            )
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an archived project.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildArchivedProjectMessage(string? projectName)
    {
        return "archived project " + (projectName ?? "<Unknown Project>");
    }

    /// <summary>
    /// Builds a message for an unarchived project.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUnArchivedProjectMessage(string? projectName)
    {
        return "unarchived project " + (projectName ?? "<Unknown Project>");
    }

    /// <summary>
    /// Builds a message for an added project plugin.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedProjectPluginMessage(
        string? projectName,
        List<LogChange>? changes
    )
    {
        var message = "added a new plugin to project " + (projectName ?? "<Unknown Project>");
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.NewValue}")
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an updated project plugin.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedProjectPluginMessage(
        string? projectName,
        List<LogChange>? changes
    )
    {
        var message =
            "updated plugin properties in project " + (projectName ?? "<Unknown Project>") + ": ";
        if (changes == null)
        {
            return message;
        }
        message += string.Join(
            ", ",
            changes.Select(change =>
                $" set {change.Property} from {change.OldValue} to {change.NewValue}"
            )
        );
        return message;
    }

    /// <summary>
    /// Builds a message for a removed project plugin.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedProjectPluginMessage(
        string? projectName,
        List<LogChange>? changes
    )
    {
        var message = "removed a plugin from project " + (projectName ?? "<Unknown Project>");
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.OldValue}")
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an added user.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedUserMessage(List<LogChange>? changes)
    {
        var message = "added a new user";
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.NewValue}")
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an updated user.
    /// </summary>
    /// <param name="log">The log entry.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedUserMessage(Log log)
    {
        var affectedUserEmail =
            log.AffectedUser?.Email ?? log.AffectedUserEmail ?? "<Unknown User>";
        affectedUserEmail = GetNameFromEmail(affectedUserEmail);

        var message = $"updated user {affectedUserEmail}: ";
        message += string.Join(
            ", ",
            log.Changes!.Select(change =>
                change.Property switch
                {
                    nameof(ApplicationUser.PasswordHash) => "changed password",
                    _ => $"set {change.Property} from {change.OldValue} to {change.NewValue}",
                }
            )
        );
        return message;
    }

    /// <summary>
    /// Builds a message for a removed user.
    /// </summary>
    /// <param name="username">The username of the removed user.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedUserMessage(string username)
    {
        return "removed user " + username;
    }

    /// <summary>
    /// Builds a message for a removed project.
    /// </summary>
    /// <param name="projectName">The name of the removed project.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedProjectMessage(string projectName)
    {
        return "removed project " + projectName;
    }

    /// <summary>
    /// Builds a message for an added global plugin.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedGlobalPluginMessage(List<LogChange>? changes)
    {
        var message = "added a new global plugin";
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.NewValue}")
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an updated global plugin.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <param name="pluginName">The name of the updated plugin.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedGlobalPluginMessage(
        List<LogChange>? changes,
        string pluginName
    )
    {
        var message = $"updated global plugin {pluginName}: ";
        if (changes == null)
        {
            return message;
        }
        message += string.Join(
            ", ",
            changes.Select(change =>
                $"set {change.Property} from {change.OldValue} to {change.NewValue}"
            )
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an archived global plugin.
    /// </summary>
    /// <param name="pluginName">The name of the archived global plugin.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildArchivedGlobalPluginMessage(string pluginName)
    {
        return "archived global plugin " + pluginName;
    }

    /// <summary>
    /// Builds a message for an unarchived global plugin.
    /// </summary>
    /// <param name="pluginName">The name of the unarchived global plugin.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUnArchivedGlobalPluginMessage(string pluginName)
    {
        return "unarchived global plugin " + pluginName;
    }

    /// <summary>
    /// Builds a message for a removed global plugin.
    /// </summary>
    /// <param name="pluginName">The name of the removed global plugin.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedGlobalPluginMessage(string pluginName)
    {
        return "removed global plugin " + pluginName;
    }

    /// <summary>
    /// Build a message for an added api token.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedApiTokenMessage(List<LogChange>? changes)
    {
        var message = "created a new API token";
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.NewValue}")
        );
        return message;
    }

    /// <summary>
    /// Build a message for an removed api token.
    /// </summary>
    /// <param name="changes">List of changes.</param>
    /// <param name="tokenName">Name of the token.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedApiTokenMessage(List<LogChange>? changes, string tokenName)
    {
        var message = "removed the API token " + tokenName;
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.OldValue}")
        );
        return message;
    }

    /// <summary>
    /// Builds a message for regenerating a api token.
    /// </summary>
    /// <param name="tokenName"></param>
    /// <returns></returns>
    private static string BuildRegeneratedApiTokenMessage(string tokenName)
    {
        return "regenerated the API token " + tokenName;
    }

    /// <summary>
    /// Builds a message for an added office location.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedOfficeLocationMessage(List<LogChange>? changes)
    {
        var message = "added a new office location";
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.NewValue}")
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an updated office location.
    /// </summary>
    /// <param name="log">The log entry.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedOfficeLocationMessage(Log log)
    {
        var message = $"updated office location {log.OfficeLocationName}: ";
        message += string.Join(
            ", ",
            log.Changes!.Select(change =>
                $"set {change.Property} from {change.OldValue} to {change.NewValue}"
            )
        );
        return message;
    }

    /// <summary>
    /// Builds a message for a removed office location.
    /// </summary>
    /// <param name="officeLocationName">The office location name of the removed office location.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedOfficeLocationMessage(string officeLocationName)
    {
        return "removed office location " + officeLocationName;
    }

    /// <summary>
    /// Builds a message for an added company.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedCompanyMessage(List<LogChange>? changes)
    {
        var message = "added a new company";
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.NewValue}")
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an updated company.
    /// </summary>
    /// <param name="log">The log entry.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedCompanyMessage(Log log)
    {
        var message = $"updated company {log.CompanyName}: ";
        message += string.Join(
            ", ",
            log.Changes!.Select(change =>
                $"set {change.Property} from {change.OldValue} to {change.NewValue}"
            )
        );
        return message;
    }

    /// <summary>
    /// Builds a message for a removed company.
    /// </summary>
    /// <param name="companyName">The company name of the removed company.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedCompanyMessage(string companyName)
    {
        return "removed company " + companyName;
    }

    /// <summary>
    /// Builds a message for an added department.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedDepartmentMessage(List<LogChange>? changes)
    {
        var message = "added a new department";
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.NewValue}")
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an updated department.
    /// </summary>
    /// <param name="log">The log entry.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedDepartmentMessage(Log log)
    {
        var message = $"updated department {log.DepartmentName}: ";
        message += string.Join(
            ", ",
            log.Changes!.Select(change =>
                $"set {change.Property} from {change.OldValue} to {change.NewValue}"
            )
        );
        return message;
    }

    /// <summary>
    /// Builds a message for a removed department.
    /// </summary>
    /// <param name="departmentName">The department name of the removed department.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedDepartmentMessage(string departmentName)
    {
        return "removed department " + departmentName;
    }

    /// <summary>
    /// Builds a message for an added businessUnit.
    /// </summary>
    /// <param name="changes">The list of changes.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildAddedBusinessUnitMessage(List<LogChange>? changes)
    {
        var message = "added a new businessUnit";
        if (changes == null)
        {
            return message;
        }
        message += " with properties: ";
        message += string.Join(
            ", ",
            changes.Select(change => $"{change.Property} = {change.NewValue}")
        );
        return message;
    }

    /// <summary>
    /// Builds a message for an updated businessUnit.
    /// </summary>
    /// <param name="log">The log entry.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildUpdatedBusinessUnitMessage(Log log)
    {
        var message = $"updated businessUnit {log.BusinessUnitName}: ";
        message += string.Join(
            ", ",
            log.Changes!.Select(change =>
                $"set {change.Property} from {change.OldValue} to {change.NewValue}"
            )
        );
        return message;
    }

    /// <summary>
    /// Builds a message for a removed businessUnit.
    /// </summary>
    /// <param name="businessUnitName">The businessUnit name of the removed businessUnit.</param>
    /// <returns>The constructed message.</returns>
    private static string BuildRemovedBusinessUnitMessage(string businessUnitName)
    {
        return "removed businessUnit " + businessUnitName;
    }

    /// <summary>
    /// Gets the timestamp in a specific format.
    /// </summary>
    /// <param name="value">The DateTimeOffset value.</param>
    /// <returns>The formatted timestamp.</returns>
    private static string GetTimestamp(DateTimeOffset value)
    {
        return value.ToString("yyyy-MM-ddTHH:mm:ssK");
    }

    /// <summary>
    /// Extracts the Name out of the email of a user.
    /// </summary>
    /// <param name="email">Email to extract the name from.</param>
    /// <returns>The input if there is not @ in the string or the extracted name.</returns>
    private static string GetNameFromEmail(String email)
    {
        var splitEmail = email.Split("@");
        return splitEmail.Length == 2 ? splitEmail[0] : email;
    }
}

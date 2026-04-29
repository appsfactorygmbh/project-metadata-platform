using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Handles the command to patch user information.
/// </summary>
public class PatchUserCommandHandler : IRequestHandler<PatchUserCommand, ApplicationUser>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ITeamRepository _teamRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogRepository _logRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchUserCommandHandler"/> class.
    /// </summary>
    /// <param name="usersRepository">The repository for accessing user data.</param>
    /// <param name="passwordHasher">The service for hashing user passwords.</param>
    /// <param name="teamRepository">The repository for accessing team data.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="logRepository">The repository for logging user actions.</param>
    public PatchUserCommandHandler(
        IUsersRepository usersRepository,
        IPasswordHasher<ApplicationUser> passwordHasher,
        ITeamRepository teamRepository,
        IUnitOfWork unitOfWork,
        ILogRepository logRepository
    )
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _teamRepository = teamRepository;
        _unitOfWork = unitOfWork;
        _logRepository = logRepository;
    }

    /// <summary>
    /// Handles the patch user command.
    /// </summary>
    /// <param name="request">The command containing the user information to be patched.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated user information, or null if the user was not found.</returns>
    public async Task<ApplicationUser> Handle(
        PatchUserCommand request,
        CancellationToken cancellationToken
    )
    {
        ApplicationUser user;
        if (await _usersRepository.CheckUserExists(request.Id))
        {
            user = await _usersRepository.GetUserByIdAsync(request.Id);
        }
        else
        {
            user = await _usersRepository.GetUserByEmailAsync(request.Id);
        }

        var changes = new List<LogChange>();
        await UpdateUser(request, user, changes);

        var response = await _usersRepository.StoreUser(user);

        if (changes.Count > 0)
        {
            await _logRepository.AddUserLogForCurrentActor(user, Action.UPDATED_USER, changes);
        }

        await _unitOfWork.CompleteAsync();
        return response;
    }

    /// <summary>
    /// Method for updating all relevent user attributes.
    /// </summary>
    /// <param name="request">The command containing the user information to be patched.</param>
    /// <param name="user">User that should be updated.</param>
    /// <param name="changes">List of Changes for logging purposes.</param>
    /// <returns></returns>
    private async Task UpdateUser(
        PatchUserCommand request,
        ApplicationUser user,
        List<LogChange> changes
    )
    {
        var properties = typeof(ApplicationUser).GetProperties();
        foreach (var operation in request.Operations)
        {
            var attribute = properties.Select(prop => prop.Name).Contains(operation.Path)
                ? operation.Path
                : await ScimAttributeNameToTypeAttributeName(operation.Path);
            var oldValue = user.GetType().GetProperty(attribute)?.GetValue(user);
            if (attribute == "Email" && operation.Operation != PatchOperations.Remove)
            {
                await UpdateEmail(user, changes, operation, oldValue);
            }
            else if (
                attribute == "PasswordHash"
                && operation.Operation != PatchOperations.Remove
                && await _usersRepository.CheckPasswordFormat(
                    (await JsonElementToPrimitive((JsonElement)operation.Value!) as string)!
                )
            )
            {
                await UpdatePassword(user, changes, operation, oldValue);
            }
            else if (attribute == "Teams" || attribute == "TeamSupport")
            {
                await UpdateTeamPropery(user, changes, operation, attribute, oldValue);
            }
            else
            {
                var type = properties
                    .Where(prop => prop.Name == attribute)
                    .Select(prop => prop.PropertyType)
                    .FirstOrDefault(typeof(object));
                await UpdateProperty(user, changes, operation, attribute, type, oldValue);
            }
        }
    }

    /// <summary>
    /// Updates the users email and username.
    /// </summary>
    /// <param name="user">user that is being changed.</param>
    /// <param name="changes">List of Changes for logging purposes.</param>
    /// <param name="operation">Email Update Operation.</param>
    /// <param name="oldValue">old email value.</param>
    /// <returns></returns>
    private static async Task UpdateEmail(
        ApplicationUser user,
        List<LogChange> changes,
        PatchUserCommand.OperationRecord operation,
        object? oldValue
    )
    {
        user.Email = await JsonElementToPrimitive((JsonElement)operation.Value!) as string;
        user.UserName = await JsonElementToPrimitive((JsonElement)operation.Value!) as string;

        if ((string?)oldValue != user.Email)
        {
            changes.Add(
                new LogChange
                {
                    OldValue = oldValue?.ToString() ?? "null",
                    NewValue =
                        (await JsonElementToPrimitive((JsonElement)operation.Value!))?.ToString()
                        ?? "null",
                    Property = nameof(ApplicationUser.Email),
                }
            );
        }
    }

    /// <summary>
    /// Updated the user password.
    /// </summary>
    /// <param name="user">user that is being changed.</param>
    /// <param name="changes">List of Changes for logging purposes.</param>
    /// <param name="operation">Password Update Operation.</param>
    /// <param name="oldValue">old passwordhash value.</param>
    /// <returns></returns>
    private async Task UpdatePassword(
        ApplicationUser user,
        List<LogChange> changes,
        PatchUserCommand.OperationRecord operation,
        object? oldValue
    )
    {
        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            (await JsonElementToPrimitive((JsonElement)operation.Value!) as string)!
        );

        if ((string?)oldValue != user.PasswordHash)
        {
            changes.Add(
                new LogChange
                {
                    OldValue = oldValue == null ? "null" : "old password was changed",
                    NewValue = operation.Value?.ToString() == null ? "null" : "new password *****",
                    Property = nameof(ApplicationUser.PasswordHash),
                }
            );
        }
    }

    /// <summary>
    /// Updates the user teams or teamsupport.
    /// </summary>
    /// <param name="user">user that is being changed.</param>
    /// <param name="changes">List of Changes for logging purposes.</param>
    /// <param name="operation">Team Update Operation.</param>
    /// <param name="attribute">Name of the attribute that is being changed.</param>
    /// <param name="oldValue">old teams or teamsupport value.</param>
    /// <returns></returns>
    private async Task UpdateTeamPropery(
        ApplicationUser user,
        List<LogChange> changes,
        PatchUserCommand.OperationRecord operation,
        string attribute,
        object? oldValue
    )
    {
        List<string> teamNames = [];
        if (operation.Operation != PatchOperations.Remove)
        {
            teamNames = (
                await JsonElementToPrimitive((JsonElement)operation.Value!) as List<string>
            )!;
        }
        Collection<Team> teams = [];

        foreach (var team in teamNames)
        {
            var teamObject = await _teamRepository.GetTeamByNameAsync(team);

            teams.Add(teamObject);
        }
        user.GetType().GetProperty(attribute)?.SetValue(user, teams);
        var oldValueNamesList =
            oldValue == null ? [] : ((HashSet<Team>)oldValue).Select(t => t.TeamName);
        if (!oldValueNamesList.SequenceEqual(teamNames))
        {
            changes.Add(
                new LogChange
                {
                    OldValue = oldValueNamesList.Any()
                        ? "[" + string.Join(", ", oldValueNamesList) + "]"
                        : "null",
                    NewValue = teamNames.Any() ? "[" + string.Join(", ", teamNames) + "]" : "null",
                    Property = attribute,
                }
            );
        }
    }

    /// <summary>
    /// Updates any Property of the User
    /// </summary>
    /// <param name="user">User that is being updated.</param>
    /// <param name="changes">List of changes for Logging purposes.</param>
    /// <param name="operation">One Update Operation.</param>
    /// <param name="attribute">Attribute that is being changed.</param>
    /// <param name="oldValue">Pre Update value of the attribute.</param>
    /// <returns></returns>
    private static async Task UpdateProperty(
        ApplicationUser user,
        List<LogChange> changes,
        PatchUserCommand.OperationRecord operation,
        string attribute,
        Type type,
        object? oldValue
    )
    {
        user.GetType()
            .GetProperty(attribute)
            ?.SetValue(
                user,
                operation.Operation == PatchOperations.Remove
                    ? null
                    : await JsonElementToPrimitive((JsonElement)operation.Value!)
            );

        if (type == typeof(List<string>))
        {
            await AddListPropertyLogChange(changes, operation, attribute, oldValue);
        }
        else
        {
            await AddSinglePropertyLogChange(user, changes, operation, attribute, oldValue);
        }
    }

    private static async Task AddSinglePropertyLogChange(
        ApplicationUser user,
        List<LogChange> changes,
        PatchUserCommand.OperationRecord operation,
        string attribute,
        object? oldValue
    )
    {
        if (
            (oldValue?.ToString() ?? "null")
            != (user.GetType().GetProperty(attribute)?.GetValue(user)?.ToString() ?? "null")
        )
        {
            changes.Add(
                new LogChange
                {
                    OldValue = oldValue?.ToString() ?? "null",
                    NewValue =
                        operation.Operation == PatchOperations.Remove
                            ? "null"
                            : (
                                await JsonElementToPrimitive((JsonElement)operation.Value!)
                            )?.ToString() ?? "null",
                    Property = attribute,
                }
            );
        }
    }

    private static async Task AddListPropertyLogChange(
        List<LogChange> changes,
        PatchUserCommand.OperationRecord operation,
        string attribute,
        object? oldValue
    )
    {
        var oldValueList = (oldValue as List<string>) ?? [];
        var valueList =
            (await JsonElementToPrimitive((JsonElement)operation.Value!) as List<string>) ?? [];

        if (!oldValueList.SequenceEqual(valueList))
        {
            changes.Add(
                new LogChange
                {
                    OldValue = !oldValueList.Any()
                        ? "null"
                        : "[" + string.Join(", ", oldValueList) + "]",
                    NewValue =
                        operation.Operation == PatchOperations.Remove || !valueList.Any()
                            ? "null"
                            : "[" + string.Join(", ", valueList) + "]",
                    Property = attribute,
                }
            );
        }
    }

    /// <summary>
    /// Returns the fitting ApplicationUser Attribute for the given scim attribute name.
    /// </summary>
    /// <param name="attributeName">Name of the scim attribute</param>
    /// <returns>Name of the Application User attribute</returns>
    /// <exception cref="NotSupportedException">Thrown if the given AttributeName has no equivalent in the ApplicationUser class,</exception>
    private static async Task<string> ScimAttributeNameToTypeAttributeName(string attributeName)
    {
        return attributeName switch
        {
            "id" or "externalId" => "EmployeeId",
            "userName" => "Email",
            "active" => "IsActive",
            "password" => "PasswordHash",
            "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:organization" => "Company",
            "urn:ietf:params:scim:schemas:extension:pmp:User:departments" => "Departments",
            "urn:ietf:params:scim:schemas:extension:pmp:User:teamSupport" => "TeamSupport",
            "urn:ietf:params:scim:schemas:extension:pmp:User:jobTitles" => "JobTitles",
            "urn:ietf:params:scim:schemas:extension:pmp:User:team" => "Teams",
            "urn:ietf:params:scim:schemas:extension:pmp:User:businessUnits" => "BusinessUnits",
            _ => throw new NotSupportedException("Unsupported Attribute Name"),
        };
    }

    /// <summary>
    /// Converts an object of the type JsonElement to its c# primitive type.
    /// </summary>
    /// <param name="value">JsonElement that should be converted.</param>
    /// <returns>Value as a C# primitive type.</returns>
    /// <exception cref="NotSupportedException">Thrown if the Type of value is unsupported in the current implementation.</exception>
    private static async Task<object?> JsonElementToPrimitive(JsonElement value)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.String:
                var stringValue = value.GetString();
                if (bool.TryParse(stringValue, out var boolValue))
                {
                    return boolValue;
                }
                return stringValue;

            case JsonValueKind.Number:
                return value.GetInt32();

            case JsonValueKind.True:
            case JsonValueKind.False:
                return value.GetBoolean();
            //! Only handles string arrays for now
            case JsonValueKind.Array:
                List<string> list = [];

                foreach (var element in value.EnumerateArray())
                {
                    list.Add((await JsonElementToPrimitive(element) as string)!);
                }
                return list;

            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            // Only handles objects of the type:
            // {
            // "value": "objectValue",
            // }
            case JsonValueKind.Object:
                return await JsonElementToPrimitive(value.GetProperty("value"));
            default:
                throw new NotSupportedException("Unsupported Value Type");
        }
    }
}

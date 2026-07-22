using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Handler for Query for retrieving all users.
/// </summary>
public class GetAllUsersQueryHandler
    : IRequestHandler<
        GetAllUsersQuery,
        (IEnumerable<ApplicationUser>, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly IUsersRepository _usersRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllUsersQueryHandler" />.
    /// </summary>
    public GetAllUsersQueryHandler(
        IUsersRepository usersRepository,
        IAuthorizationService authorizationService
    )
    {
        _usersRepository = usersRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the request to retrieve all filtered users.
    /// </summary>
    public async Task<(
        IEnumerable<ApplicationUser>,
        IEnumerable<AuthorizationConstants.Actions>
    )> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _usersRepository.GetUsersAsync(request.Filter);
        var queriedUsers = await _authorizationService.TryGetPlanResourceQuery(users);
        var permissions = await _authorizationService.GetPermissions<ApplicationUser>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        if (queriedUsers == null)
        {
            List<ApplicationUser> filteredUsers = [];
            foreach (var user in users)
            {
                if (
                    await _authorizationService.CheckAccess(
                        user,
                        AuthorizationConstants.Actions.GET
                    )
                )
                {
                    filteredUsers.Add(user);
                }
            }
            return (filteredUsers, permissions);
        }
        return (await queriedUsers.ToListAsync(cancellationToken: cancellationToken), permissions);
    }
}

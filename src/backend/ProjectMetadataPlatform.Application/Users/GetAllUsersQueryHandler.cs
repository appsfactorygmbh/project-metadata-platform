using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Auth;
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
        (IEnumerable<ApplicationUser>, IEnumerable<AuthorizationConstants.Actions>, UserCursor?)
    >
{
    private readonly IUsersRepository _usersRepository;
    private readonly IAuthorizationService _authorizationService;

    private readonly IPaginationHelper _paginationHelper;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllUsersQueryHandler" />.
    /// </summary>
    public GetAllUsersQueryHandler(
        IUsersRepository usersRepository,
        IAuthorizationService authorizationService,
        IPaginationHelper paginationHelper
    )
    {
        _usersRepository = usersRepository;
        _authorizationService = authorizationService;
        _paginationHelper = paginationHelper;
    }

    /// <summary>
    /// Handles the request to retrieve all filtered users.
    /// </summary>
    public async Task<(
        IEnumerable<ApplicationUser>,
        IEnumerable<AuthorizationConstants.Actions>,
        UserCursor?
    )> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _usersRepository.GetUsersAsync(request.Filter, request.Cursor);
        var (paginatedUsers, lastUser) = await _paginationHelper.PaginateWithAuthAsync(
            users,
            request.Limit
        );
        var permissions = await _authorizationService.GetAllowedActions<ApplicationUser>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        var nextCursor =
            lastUser == null ? null : new UserCursor(lastUser.Email!, lastUser.EmployeeId);
        return (paginatedUsers, permissions, nextCursor);
    }
}

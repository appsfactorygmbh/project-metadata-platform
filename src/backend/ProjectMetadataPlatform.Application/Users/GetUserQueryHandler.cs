using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Handles the <see cref="GetUserQuery"/>.
/// </summary>
public class GetUserQueryHandler
    : IRequestHandler<GetUserQuery, (ApplicationUser, IEnumerable<AuthorizationConstants.Actions>)>
{
    /// <summary>
    /// The repository of users.
    /// </summary>
    private readonly IUsersRepository _usersRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserQueryHandler"/> class.
    /// </summary>
    /// <param name="usersRepository">The repository of users.</param>
    /// <param name="authorizationService"></param>
    public GetUserQueryHandler(
        IUsersRepository usersRepository,
        IAuthorizationService authorizationService
    )
    {
        _usersRepository = usersRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the GetUserQuery.
    /// </summary>
    /// <param name="request">The GetUserQuery.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user with the specified ID and allowed actions, or null if no user is found.</returns>
    public async Task<(ApplicationUser, IEnumerable<AuthorizationConstants.Actions>)> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken
    )
    {
        var user = await _usersRepository.GetUserByIdAsync(request.EmployeeId);
        if (!await _authorizationService.CheckAccess(user, AuthorizationConstants.Actions.GET))
        {
            throw new UnauthorizedException();
        }
        var permissions = await _authorizationService.GetPermissions(user);
        return (user, permissions);
    }
}

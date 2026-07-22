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
/// Handles the query to get a user by their email.
/// </summary>
public class GetUserByEmailQueryHandler
    : IRequestHandler<
        GetUserByEmailQuery,
        (ApplicationUser, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly IUsersRepository _usersRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByEmailQueryHandler"/> class.
    /// </summary>
    /// <param name="usersRepository">The repository for accessing user data.</param>
    /// <param name="authorizationService"></param>
    public GetUserByEmailQueryHandler(
        IUsersRepository usersRepository,
        IAuthorizationService authorizationService
    )
    {
        _usersRepository = usersRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the query to get a user by their email.
    /// </summary>
    /// <param name="request">The query request containing the email.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The user with the specified email, or null if not found.</returns>
    public async Task<(ApplicationUser, IEnumerable<AuthorizationConstants.Actions>)> Handle(
        GetUserByEmailQuery request,
        CancellationToken cancellationToken
    )
    {
        var user = await _usersRepository.GetUserByEmailAsync(request.Email);
        if (!await _authorizationService.CheckAccess(user, AuthorizationConstants.Actions.GET))
        {
            throw new UnauthorizedException();
        }
        var permissions = await _authorizationService.GetPermissions(user,[AuthorizationConstants.Actions.EDIT,AuthorizationConstants.Actions.DELETE]);
        return (user, permissions);
    }
}

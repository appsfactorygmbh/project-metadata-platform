using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Handles the <see cref="GetUserQuery"/>.
/// </summary>
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, IdentityUser>
{
    /// <summary>
    /// The repository of users.
    /// </summary>
    private readonly IUsersRepository _usersRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserQueryHandler"/> class.
    /// </summary>
    /// <param name="usersRepository">The repository of users.</param>
    public GetUserQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    /// <summary>
    /// Handles the GetUserQuery.
    /// </summary>
    /// <param name="request">The GetUserQuery.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user with the specified ID, or null if no user is found.</returns>
    public Task<IdentityUser> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return _usersRepository.GetUserByIdAsync(request.UserId);
    }
}

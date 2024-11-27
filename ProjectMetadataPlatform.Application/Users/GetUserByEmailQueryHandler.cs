using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Handles the query to get a user by their email.
/// </summary>
public class GetUserByEmailQueryHandler: IRequestHandler<GetUserByEmailQuery, User?>
{
    private readonly IUsersRepository _usersRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByEmailQueryHandler"/> class.
    /// </summary>
    /// <param name="usersRepository">The repository for accessing user data.</param>
    public GetUserByEmailQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    /// <summary>
    /// Handles the query to get a user by their email.
    /// </summary>
    /// <param name="request">The query request containing the email.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The user with the specified email, or null if not found.</returns>
    public Task<User?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        return _usersRepository.GetUserByEmailAsync(request.Email);
    }
}

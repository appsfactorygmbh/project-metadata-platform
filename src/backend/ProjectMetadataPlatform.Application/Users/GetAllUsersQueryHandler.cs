using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Query for retrieving all users.
/// </summary>
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<IdentityUser>>
{
    private readonly IUsersRepository _usersRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllUsersQueryHandler" />.
    /// </summary>
    public GetAllUsersQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    /// <summary>
    /// Handles the request to retrieve all users.
    /// </summary>
    public async Task<IEnumerable<IdentityUser>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _usersRepository.GetAllUsersAsync();
    }
}

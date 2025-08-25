using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Handles the command to patch user information.
/// </summary>
public class PatchUserCommandHandler : IRequestHandler<PatchUserCommand, IdentityUser>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher<IdentityUser> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogRepository _logRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchUserCommandHandler"/> class.
    /// </summary>
    /// <param name="usersRepository">The repository for accessing user data.</param>
    /// <param name="passwordHasher">The service for hashing user passwords.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="logRepository">The repository for logging user actions.</param>
    public PatchUserCommandHandler(
        IUsersRepository usersRepository,
        IPasswordHasher<IdentityUser> passwordHasher,
        IUnitOfWork unitOfWork,
        ILogRepository logRepository
    )
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logRepository = logRepository;
    }

    /// <summary>
    /// Handles the patch user command.
    /// </summary>
    /// <param name="request">The command containing the user information to be patched.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated user information, or null if the user was not found.</returns>
    public async Task<IdentityUser> Handle(
        PatchUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _usersRepository.GetUserByIdAsync(request.Id);

        var oldEmail = string.Empty;
        if (request.Email != null)
        {
            oldEmail = user.Email ?? string.Empty;
        }
        user.Email = request.Email ?? user.Email;
        user.UserName = user.Email;

        var oldPasswordHash = user.PasswordHash ?? string.Empty;
        if (
            request.Password != null
            && await _usersRepository.CheckPasswordFormat(request.Password)
        )
        {
            user.PasswordHash =
                request.Password != null
                    ? _passwordHasher.HashPassword(user, request.Password)
                    : user.PasswordHash;
        }

        var response = await _usersRepository.StoreUser(user);

        var changes = new List<LogChange>();

        if (request.Email != null && oldEmail != request.Email)
        {
            changes.Add(
                new LogChange
                {
                    OldValue = oldEmail,
                    NewValue = request.Email,
                    Property = nameof(IdentityUser.Email),
                }
            );
        }

        if (oldPasswordHash != user.PasswordHash && request.Password != null)
        {
            changes.Add(
                new LogChange
                {
                    OldValue = "old password was changed",
                    NewValue = "new password *****",
                    Property = nameof(IdentityUser.PasswordHash),
                }
            );
        }

        if (changes.Count > 0)
        {
            await _logRepository.AddUserLogForCurrentUser(user, Action.UPDATED_USER, changes);
        }

        await _unitOfWork.CompleteAsync();
        return response;
    }
}

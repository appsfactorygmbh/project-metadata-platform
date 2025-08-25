using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.UserException;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Handles the command to delete a user by their unique identifier.
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IdentityUser>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteUserCommandHandler"/> class.
    /// </summary>
    /// <param name="usersRepository">The users repository.</param>
    /// <param name="httpContextAccessor">Provides Access to the current Http Context.</param>
    /// <param name="logRepository">The log repository.</param>
    /// <param name="unitOfWork">Unit of Work</param>
    public DeleteUserCommandHandler(
        IUsersRepository usersRepository,
        IHttpContextAccessor httpContextAccessor,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _usersRepository = usersRepository;
        _httpContextAccessor = httpContextAccessor;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the <see cref="DeleteUserCommand"/> request.
    /// Deletes the user with the specified ID, if present. Returns the deleted user, if present, otherwise null.
    /// Throws an <see cref="UserCantDeleteThemselfException"/> if the active user tries to delete themselves.
    /// On successful deletion, a corresponding log entry is added.
    /// Also, all logs associated with the deleted user are updated, setting the email to the deleted user's current email.
    /// </summary>
    /// <param name="request">The command request containing the user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deleted user, if present, otherwise null.</returns>
    /// <exception cref="UserCantDeleteThemselfException">If the active user tries to delete themselves.</exception>
    public async Task<IdentityUser> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _usersRepository.GetUserByIdAsync(request.Id);
        var email =
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
            ?? "Unknown user";
        var activeUser = await _usersRepository.GetUserByEmailAsync(email);

        if (user == activeUser)
        {
            throw new UserCantDeleteThemselfException();
        }

        var change = new LogChange
        {
            OldValue = user.Email!,
            NewValue = "",
            Property = nameof(IdentityUser.Email),
        };
        await _logRepository.AddUserLogForCurrentUser(user, Action.REMOVED_USER, [change]);

        var response = await _usersRepository.DeleteUserAsync(user);
        await _unitOfWork.CompleteAsync();

        return response;
    }
}

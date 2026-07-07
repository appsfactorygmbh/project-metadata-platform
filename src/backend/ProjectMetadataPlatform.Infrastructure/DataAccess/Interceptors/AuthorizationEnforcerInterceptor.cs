using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.Interceptors;

/// <summary>
/// Interceptor for ceccking if authorization was checked.
/// </summary>
public class AuthorizationEnforcerInterceptor : SaveChangesInterceptor
{
    private readonly IAuthorizationTracker _tracker;

    /// <summary>
    /// Constructor for <see cref="AuthorizationEnforcerInterceptor"/>
    /// </summary>
    /// <param name="tracker">Authorization Tracker.</param>
    public AuthorizationEnforcerInterceptor(IAuthorizationTracker tracker)
    {
        _tracker = tracker;
    }

    /// <summary>
    /// Checks if Authorization was checked.
    /// </summary>
    /// <param name="eventData">Event Data.</param>
    /// <param name="result">Interception Result.</param>
    /// <returns>Interception Result.</returns>
    /// <exception cref="UnauthorizedException">Thrown if Authorization wasnt checked.</exception>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        if (_tracker.WasChecked)
        {
            return base.SavingChanges(eventData, result);
        }
        else
        {
            throw new UnauthorizedException();
        }
    }

    /// <summary>
    /// Checks if Authorization was checked
    /// </summary>
    /// <param name="eventData">Event Data.</param>
    /// <param name="result">Interception Result.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>Interception Result.</returns>
    /// <exception cref="UnauthorizedException">Thrown if Authorization wasnt checked.</exception>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (_tracker.WasChecked)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        else
        {
            throw new UnauthorizedException();
        }
    }
}

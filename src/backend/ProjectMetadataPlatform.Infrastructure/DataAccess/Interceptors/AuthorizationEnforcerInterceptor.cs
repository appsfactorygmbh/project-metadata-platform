using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.Interceptors;

public class AuthorizationEnforcerInterceptor : SaveChangesInterceptor
{
    private readonly IAuthorizationTracker _tracker;

    public AuthorizationEnforcerInterceptor(IAuthorizationTracker tracker)
    {
        _tracker = tracker;
    }

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

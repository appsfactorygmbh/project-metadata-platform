using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProjectMetadataPlatform.Application.Authorization;
using ProjectMetadataPlatform.Application.Helper;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application;

/// <summary>
/// Methods for dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the necessary dependencies for the application layer.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection with the add dependencies.</returns>
    public static IServiceCollection AddApplicationDependencies(
        this IServiceCollection serviceCollection
    )
    {
        _ = serviceCollection.AddScoped<ISlugHelper, SlugHelper>();
        _ = serviceCollection.AddScoped<IGetOrCreateHelper, GetOrCreateHelper>();

        _ = serviceCollection.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(AuthorizationEnforcerBehavior<,>)
        );
        _ = serviceCollection.AddCustomMediator(typeof(DependencyInjection).Assembly);
        return serviceCollection;
    }

    private static IServiceCollection AddCustomMediator(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        _ = services.AddTransient<IMediator, Mediator.Mediator>();

        var handlerType = typeof(IRequestHandler<,>);

        var handlers = assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t =>
                t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType)
            );

        foreach (var handler in handlers)
        {
            var implementedInterfaces = handler
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType);

            foreach (var interfaceType in implementedInterfaces)
            {
                _ = services.AddTransient(interfaceType, handler);
            }
        }

        return services;
    }
}

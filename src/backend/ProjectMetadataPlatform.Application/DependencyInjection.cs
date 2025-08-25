using Microsoft.Extensions.DependencyInjection;
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
        _ = serviceCollection.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection))
        );
        return serviceCollection;
    }
}

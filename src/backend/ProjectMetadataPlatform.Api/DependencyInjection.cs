using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Api.Logs;
using ProjectMetadataPlatform.Domain.Errors;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Errors.UserException;

namespace ProjectMetadataPlatform.Api;

/// <summary>
/// Methods for dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the necessary dependencies for the api layer.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection with the added dependencies.</returns>
    public static IServiceCollection AddApiDependencies(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddScoped<ILogConverter, LogConverter>();
        _ = serviceCollection.AddScoped<IExceptionHandler<PmpException>, BasicExceptionHandler>();
        _ = serviceCollection.AddScoped<
            IExceptionHandler<ProjectException>,
            ProjectsExceptionHandler
        >();
        _ = serviceCollection.AddScoped<
            IExceptionHandler<PluginException>,
            PluginsExceptionHandler
        >();
        _ = serviceCollection.AddScoped<IExceptionHandler<LogException>, LogsExceptionHandler>();
        _ = serviceCollection.AddScoped<IExceptionHandler<TeamException>, TeamExceptionHandler>();
        _ = serviceCollection.AddScoped<IExceptionHandler<AuthException>, AuthExceptionHandler>();
        _ = serviceCollection.AddScoped<IExceptionHandler<UserException>, UserExceptionHandler>();
        _ = serviceCollection
            .AddControllers(options =>
            {
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(
                        typeof(ErrorResponse),
                        StatusCodes.Status401Unauthorized
                    )
                );
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(
                        typeof(ErrorResponse),
                        StatusCodes.Status500InternalServerError
                    )
                );
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ErrorResponseFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.SelectMany(x =>
                        x.Errors.Select(e => e.ErrorMessage)
                    );

                    return new BadRequestObjectResult(
                        new ErrorResponse(
                            "The request is invalid. Errors: " + string.Join(" ", errors)
                        )
                    );
                };
            });

        return serviceCollection;
    }
}

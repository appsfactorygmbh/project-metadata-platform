using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Retry;
using ProjectMetadataPlatform.Api;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Swagger;
using ProjectMetadataPlatform.Api.Telemetry;
using ProjectMetadataPlatform.Application;
using ProjectMetadataPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var serviceName = "projet-metadata-platform";
var otlpEndpoint = new Uri("http://alloy:4317");

builder.Logging.AddProvider(new ActivityErrorLoggerProvider());

builder
    .Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithLogging(logging =>
        logging.AddConsoleExporter().AddOtlpExporter(options => options.Endpoint = otlpEndpoint)
    )
    .WithTracing(tracing =>
        tracing
            .SetErrorStatusOnException()
            .AddAspNetCoreInstrumentation(options => options.RecordException = true)
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(options => options.Endpoint = otlpEndpoint)
    )
    .WithMetrics(metrics =>
        metrics
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(options => options.Endpoint = otlpEndpoint)
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
    options.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();
    options.OperationFilter<UnauthorizedResponseOperationFilter>();

    var xmlDocFiles = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory), "*.xml").ToList();

    xmlDocFiles.ForEach(path => options.IncludeXmlComments(path, true));
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter your Bearer token like this: Bearer {your token}",
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    };
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);

    options.AddSecurityDefinition(
        "OIDC",
        new OpenApiSecurityScheme
        {
            Type = Microsoft.OpenApi.SecuritySchemeType.OpenIdConnect,
            OpenIdConnectUrl = new Uri(
                $"{EnvironmentUtils.GetEnvVarOrLoadFromFile("AZURE_AUTHORITY")}/v2.0/.well-known/openid-configuration"
            ),
        }
    );
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = [],
        [new OpenApiSecuritySchemeReference("OIDC", document)] = [],
    });
});

builder
    .Services.AddApiDependencies()
    .AddApplicationDependencies()
    .AddInfrastructureDependencies(
        new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse(
                    "You are either not logged in or do not have the necessary permissions to perform this action."
                );
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            },
        }
    );

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policyBuilder =>
        policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    )
);

builder.Services.AddResiliencePipeline(
    "DbCheck-Pipeline",
    builder =>
    {
        _ = builder.AddRetry(
            new RetryStrategyOptions() { MaxRetryAttempts = 10, Delay = TimeSpan.FromSeconds(5) }
        );
    }
);

var app = builder.Build();
await app.Services.CheckConnection();

app.Services.MigrateDatabase();
app.Services.AddAdminUser();
await app.Services.AddDefaultPolicies();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.OAuthClientId(EnvironmentUtils.GetEnvVarOrLoadFromFile("AZURE_FRONTEND_CLIENT_ID")); // Client-ID der SPA-Registrierung
    options.OAuthScopes(EnvironmentUtils.GetEnvVarOrLoadFromFile("AZURE_SCOPE")); // Vollständiger API-Bereich
    options.OAuthUsePkce();
});

app.UseHttpsRedirection();
app.UseAuthorization();

if (app.Environment.IsProduction())
{
    EnvironmentUtils.AddEnvToStaticFiles();
}
app.UseFileServer();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();

/// <summary>
/// The entry point for the application.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global - This is needed for the integration tests to work.
public partial class Program;

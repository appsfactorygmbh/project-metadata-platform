using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ProjectMetadataPlatform.Api;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Swagger;
using ProjectMetadataPlatform.Application;
using ProjectMetadataPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme,
        },
    };
    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement { { jwtSecurityScheme, Array.Empty<string>() } }
    );
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

var app = builder.Build();

app.Services.MigrateDatabase();
app.Services.AddAdminUser();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>
/// The entry point for the application.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global - This is needed for the integration tests to work.
public partial class Program;

using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class DependencyInjectionTests : TestsWithDatabase
{
    [Test]
    public void ServicesAreAddedCorrectly()
    {
        Environment.SetEnvironmentVariable("PMP_DB_USER", "test_user");
        Environment.SetEnvironmentVariable("PMP_DB_URL", "test_url");
        Environment.SetEnvironmentVariable("PMP_DB_PORT", "1");
        Environment.SetEnvironmentVariable("PMP_DB_PASSWORD", "test_password");
        Environment.SetEnvironmentVariable("PMP_DB_NAME", "test_db");
        Environment.SetEnvironmentVariable("JWT_VALID_ISSUER", "test_issuer");
        Environment.SetEnvironmentVariable("JWT_VALID_AUDIENCE", "test_audience");
        Environment.SetEnvironmentVariable(
            "JWT_ISSUER_SIGNING_KEY",
            "test_key_that_certainly_is_long_enough"
        );
        Environment.SetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_MINUTES", "1");
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddInfrastructureDependencies(new JwtBearerEvents());

        // Cant check this with the service provider because the context will try to connect to the database
        Assert.That(
            services.Any(descriptor =>
                descriptor.ServiceType == typeof(ProjectMetadataPlatformDbContext)
            )
        );

        var serviceProvider = services.BuildServiceProvider();
        Assert.Multiple(() =>
        {
            Assert.That(serviceProvider.GetService<IProjectsRepository>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<IPluginRepository>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<IAuthRepository>(), Is.Not.Null);
        });
    }

    [Test]
    [TestCase("test_password", "test_password")]
    [TestCase(null, "admin")]
    public void AdminUserIsAddedCorrectly(string? envPassword, string expectedPassword)
    {
        const string hash = "hash";
        Environment.SetEnvironmentVariable("PMP_ADMIN_PASSWORD", envPassword);
        var mockPasswordHasher = new Mock<IPasswordHasher<IdentityUser>>();
        mockPasswordHasher
            .Setup(m => m.HashPassword(It.IsAny<IdentityUser>(), expectedPassword))
            .Returns(hash);

        var mockUserManager = new Mock<UserManager<IdentityUser>>(
            new Mock<IUserStore<IdentityUser>>().Object,
            null!,
            mockPasswordHasher.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );
        var services = new ServiceCollection();
        services.AddScoped<UserManager<IdentityUser>>(_ => mockUserManager.Object);

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        services.AddScoped(_ => mockUnitOfWork.Object);

        mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(IdentityResult.Success);

        services.BuildServiceProvider().AddAdminUser();

        mockPasswordHasher.Verify(
            m =>
                m.HashPassword(
                    It.Is<IdentityUser>(u =>
                        u.UserName == "admin@admin.admin" && u.Email == "admin@admin.admin"
                    ),
                    expectedPassword
                ),
            Times.Once
        );
        mockUserManager.Verify(
            m =>
                m.CreateAsync(
                    It.Is<IdentityUser>(u =>
                        u.UserName == "admin@admin.admin"
                        && u.PasswordHash == hash
                        && u.Email == "admin@admin.admin"
                    )
                ),
            Times.Once
        );
    }
}

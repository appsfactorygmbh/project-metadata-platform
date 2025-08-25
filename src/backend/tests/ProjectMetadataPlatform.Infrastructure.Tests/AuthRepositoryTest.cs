using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Infrastructure.Auth;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

public class AuthRepositoryTest : TestsWithDatabase
{
    private ProjectMetadataPlatformDbContext _context;
    private AuthRepository _repository;
    private Mock<UserManager<IdentityUser>> _mockUserManager;

    [SetUp]
    public void Setup()
    {
        _mockUserManager = new Mock<UserManager<IdentityUser>>(
            new Mock<IUserStore<IdentityUser>>().Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );
        _context = DbContext();
        _repository = new AuthRepository(_context, _mockUserManager.Object);
        ClearData(_context);
    }

    [TearDown]
    public void TearDown()
    {
        using var context = DbContext();

        context.Database.EnsureDeleted();
    }

    [Test]
    public async Task CheckLogin_Successful_Test()
    {
        const string email = "test";
        const string password = "test";

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new IdentityUser { Email = email });
        _mockUserManager
            .Setup(m => m.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await _repository.CheckLogin(email, password);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckLogin_Unsuccessful_Test()
    {
        const string email = "test";
        const string password = "test";

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new IdentityUser { Email = email });
        _mockUserManager
            .Setup(m => m.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _repository.CheckLogin(email, password);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task StoreRefreshToken_Test()
    {
        const string email = "test";
        const string token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS", "6");

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new IdentityUser { Email = email });

        await _repository.StoreRefreshToken(email, token);
        var result = _repository.GetIf(rt => rt.User!.Email == email).FirstOrDefault();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.EqualTo(token));
    }

    [Test]
    public async Task UpdateRefreshToken_Test()
    {
        const string email = "test";
        const string token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS", "6");

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new IdentityUser { Email = email });
        await _repository.StoreRefreshToken(email, "oldToken");
        var count = _repository.GetEverything().Count();
        _context.ChangeTracker.Clear();

        await _repository.UpdateRefreshToken(email, token);

        var result = _repository.GetIf(rt => rt.User!.Email == email).FirstOrDefault();
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Token, Is.EqualTo(token));
            Assert.That(_repository.GetEverything().Count(), Is.EqualTo(count));
        });
    }

    [Test]
    public async Task CheckRefreshTokenExists_Successful_Test()
    {
        const string email = "test";
        const string token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS", "6");
        _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new IdentityUser { Email = email });
        await _repository.StoreRefreshToken(email, token);
        var result = await _repository.CheckRefreshTokenExists(email);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckRefreshTokenExists_Unsuccessful_Test()
    {
        const string email = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS", "6");
        _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((IdentityUser?)null);

        var result = await _repository.CheckRefreshTokenExists(email);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckRefreshTokenRequest_Successful_Test()
    {
        const string email = "test";
        const string token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS", "6");
        _context.Users.Add(new IdentityUser { Email = email });
        await _context.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);
        await _repository.StoreRefreshToken(email, token);
        var result = await _repository.CheckRefreshTokenRequest(token);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckRefreshTokenRequest_UnsuccessfulNoToken_Test()
    {
        const string email = "test";
        const string token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS", "6");
        _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new IdentityUser { Email = email });

        var result = await _repository.CheckRefreshTokenRequest(token);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckRefreshTokenRequest_UnsuccessfulExpired_Test()
    {
        const string email = "test";
        const string token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS", "0");
        _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new IdentityUser { Email = email });
        await _repository.StoreRefreshToken(email, token);
        var result = await _repository.CheckRefreshTokenRequest(token);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetEmailByRefreshToken_Test()
    {
        const string email = "test";
        const string token = "test";
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS", "0");
        _context.Users.Add(new IdentityUser { Email = email });
        await _context.SaveChangesAsync();

        _mockUserManager.Setup(m => m.Users).Returns(_context.Users);
        _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new IdentityUser { Email = email });
        await _repository.StoreRefreshToken(email, token);
        var result = await _repository.GetEmailByRefreshToken(token);
        Assert.That(result, Is.EqualTo(email));
    }
}

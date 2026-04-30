using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

namespace ProjectMetadataPlatform.Application.Tests.Auth;

[TestFixture]
public class LoginQueryHandlerTest
{
    private LoginQueryHandler _handler;
    private Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;

    private Mock<IUsersRepository> _mockUserRepo;

    [SetUp]
    public void Setup()
    {
        _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
        _mockUserRepo = new Mock<IUsersRepository>();
        _handler = new LoginQueryHandler(_mockRefreshTokenRepo.Object, _mockUserRepo.Object);
    }

    [Test]
    public async Task HandleLoginQueryHandler_ValidLogin_Test()
    {
        Environment.SetEnvironmentVariable("JWT_VALID_ISSUER", "test_issuer");
        Environment.SetEnvironmentVariable("JWT_VALID_AUDIENCE", "test_audience");
        Environment.SetEnvironmentVariable(
            "JWT_ISSUER_SIGNING_KEY",
            "test_key_that_certainly_is_long_enough"
        );
        Environment.SetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_MINUTES", "1");

        _mockUserRepo
            .Setup(m => m.CheckLogin(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        var request = new LoginQuery("username", "password");
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<JwtTokens>());
    }

    [Test]
    public async Task HandleLoginQueryHandler_UpdateExistingRefreshToken_Test()
    {
        Environment.SetEnvironmentVariable("JWT_VALID_ISSUER", "test_issuer");
        Environment.SetEnvironmentVariable("JWT_VALID_AUDIENCE", "test_audience");
        Environment.SetEnvironmentVariable(
            "JWT_ISSUER_SIGNING_KEY",
            "test_key_that_certainly_is_long_enough"
        );
        Environment.SetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_MINUTES", "1");

        _mockUserRepo
            .Setup(m => m.CheckLogin(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockRefreshTokenRepo
            .Setup(m => m.CheckRefreshTokenExists(It.IsAny<string>()))
            .ReturnsAsync(true);
        var request = new LoginQuery("username", "password");
        await _handler.Handle(request, It.IsAny<CancellationToken>());

        _mockRefreshTokenRepo.Verify(
            m => m.UpdateRefreshToken(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once
        );
    }

    [Test]
    public void HandleLoginQueryHandler_InvalidLogin_Test()
    {
        _mockUserRepo
            .Setup(m => m.CheckLogin(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        var request = new LoginQuery("wrong_username", "password");

        Assert.ThrowsAsync<AuthInvalidLoginCredentialsException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

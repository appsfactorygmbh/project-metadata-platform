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
public class RefreshTokenQueryHandlerTest
{
    private RefreshTokenQueryHandler _handler;
    private Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;

    [SetUp]
    public void Setup()
    {
        _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
        _handler = new RefreshTokenQueryHandler(_mockRefreshTokenRepo.Object);
    }

    [Test]
    public async Task HandleRefreshTokenQueryHandler_ValidToken_Test()
    {
        Environment.SetEnvironmentVariable("JWT_VALID_ISSUER", "test_issuer");
        Environment.SetEnvironmentVariable("JWT_VALID_AUDIENCE", "test_audience");
        Environment.SetEnvironmentVariable(
            "JWT_ISSUER_SIGNING_KEY",
            "test_key_that_certainly_is_long_enough"
        );
        Environment.SetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_MINUTES", "1");

        _ = _mockRefreshTokenRepo
            .Setup(m => m.CheckRefreshTokenRequest(It.IsAny<string>()))
            .ReturnsAsync(true);
        _ = _mockRefreshTokenRepo
            .Setup(m => m.GetEmailByRefreshToken(It.IsAny<string>()))
            .ReturnsAsync("admin");
        var request = new RefreshTokenQuery("refreshToken");
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<JwtTokens>());
    }

    [Test]
    public void HandleRefreshTokenQueryHandler_InvalidToken_Test()
    {
        _ = _mockRefreshTokenRepo
            .Setup(m => m.CheckRefreshTokenRequest(It.IsAny<string>()))
            .ReturnsAsync(false);
        var request = new RefreshTokenQuery("invalidRefreshToken");

        _ = Assert.ThrowsAsync<AuthInvalidRefreshTokenException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

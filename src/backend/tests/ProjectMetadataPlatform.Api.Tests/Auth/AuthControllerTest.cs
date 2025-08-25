using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Auth;
using ProjectMetadataPlatform.Api.Auth.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

namespace ProjectMetadataPlatform.Api.Tests.Auth;

public class Tests
{
    private AuthController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new AuthController(_mediator.Object);
    }

    [Test]
    public async Task SuccessfulLoginTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new JwtTokens { AccessToken = "accessToken", RefreshToken = "refreshToken" }
            );

        var request = new LoginRequest("username", "password");

        var result = await _controller.Post(request);
        Assert.That(result.Value, Is.InstanceOf<LoginResponse>());
        Assert.Multiple(() =>
        {
            Assert.That(result.Value.AccessToken, Is.EqualTo("accessToken"));
            Assert.That(result.Value.RefreshToken, Is.EqualTo("refreshToken"));
        });
    }

    [Test]
    public void WrongCredentialsLoginTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<LoginQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AuthInvalidLoginCredentialsException());

        var request = new LoginRequest("wrong_username", "password");

        Assert.ThrowsAsync<AuthInvalidLoginCredentialsException>(() => _controller.Post(request));
    }

    [Test]
    public async Task SuccessfulRefreshTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<RefreshTokenQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new JwtTokens { AccessToken = "accessToken", RefreshToken = "refreshToken" }
            );

        var request = "Refresh refreshToken";

        var result = await _controller.Get(request);
        Assert.That(result.Value, Is.InstanceOf<LoginResponse>());
        Assert.Multiple(() =>
        {
            Assert.That(result.Value.AccessToken, Is.EqualTo("accessToken"));
            Assert.That(result.Value.RefreshToken, Is.EqualTo("refreshToken"));
        });
    }

    [Test]
    public void InvalidRefreshTokenTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<RefreshTokenQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AuthenticationException("Invalid refresh token."));

        const string refreshToken = "Refresh invalidRefreshToken";

        Assert.ThrowsAsync<AuthenticationException>(() => _controller.Get(refreshToken));
    }

    [Test]
    public async Task InvalidHeaderTest()
    {
        const string request = "invalidHeader";
        var result = await _controller.Get(request);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestObjectResult = result.Result as BadRequestObjectResult;
        Assert.That(
            (badRequestObjectResult!.Value as ErrorResponse)!.Message,
            Is.EqualTo("Invalid Header format")
        );
    }
}

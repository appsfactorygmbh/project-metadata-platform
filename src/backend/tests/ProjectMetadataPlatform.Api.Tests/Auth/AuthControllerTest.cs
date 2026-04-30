using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    [Test]
    public async Task GetApiTokens_EmptyResponseTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllApiTokensQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var result = await _controller.GetApiTokens();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetApiTokenDetailsResponse>>());

        var getTokensResponseList = (
            okResult.Value as IEnumerable<GetApiTokenDetailsResponse>
        )!.ToList();
        Assert.That(getTokensResponseList, Is.Not.Null);

        Assert.That(getTokensResponseList, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetApiTokens_ListResponse()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllApiTokensQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new ApiToken
                {
                    Id = 1,
                    Name = "Token1",
                    Token = "Secret",
                },
                new ApiToken
                {
                    Id = 2,
                    Name = "Token2",
                    Token = "AlsoASecret",
                    ExpirationDate = DateTime.Now,
                },
            ]);
        var result = await _controller.GetApiTokens();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetApiTokenDetailsResponse>>());

        var getTokensResponseList = (
            okResult.Value as IEnumerable<GetApiTokenDetailsResponse>
        )!.ToList();
        Assert.That(getTokensResponseList, Is.Not.Null);

        Assert.That(getTokensResponseList, Has.Count.EqualTo(2));
        Assert.That(getTokensResponseList[0].Id, Is.EqualTo(1));
        Assert.That(getTokensResponseList[0].Name, Is.EqualTo("Token1"));
        Assert.That(getTokensResponseList[1].Id, Is.EqualTo(2));
        Assert.That(getTokensResponseList[1].Name, Is.EqualTo("Token2"));
    }

    [Test]
    public async Task GetApiTokens_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetAllApiTokensQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.GetApiTokens());
    }

    [Test]
    public async Task GetApiToken_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetApiTokenDetailsQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.GetApiToken(0));
    }

    [Test]
    public async Task GetApiToken_TokenResponseTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetApiTokenDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ApiToken
                {
                    Name = "Token",
                    Id = 1,
                    Scopes = [TokenScopes.SCIM],
                    ExpirationDate = new DateTimeOffset(),
                    Token = "SuperCriticalSecret",
                }
            );
        var result = await _controller.GetApiToken(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetApiTokenDetailsResponse>());

        var getTokenResponse = okResult.Value as GetApiTokenDetailsResponse;
        Assert.That(getTokenResponse, Is.Not.Null);

        Assert.That(getTokenResponse.Name, Is.EqualTo("Token"));
        Assert.That(getTokenResponse.Id, Is.EqualTo(1));
        Assert.That(
            getTokenResponse.Scopes,
            Is.EqualTo(new List<TokenScopes> { TokenScopes.SCIM })
        );
        Assert.That(getTokenResponse.ExpirationDate, Is.EqualTo(new DateTimeOffset()));
        Assert.That(getTokenResponse.Token, Is.Null);
    }

    [Test]
    public async Task PostApiToken_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateApiTokenCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.PostApiToken(new CreateApiTokenRequest("", []))
        );
    }

    [Test]
    public async Task PostApiToken_ReturnsTokenwithValueTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateApiTokenCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new ApiToken
                {
                    Name = "Token",
                    Id = 1,
                    Scopes = [],
                    ExpirationDate = new DateTimeOffset(),
                    Token = "SuperCriticalSecret",
                }
            );

        var request = new CreateApiTokenRequest("Token", []);
        var result = await _controller.PostApiToken(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Location, Is.EqualTo("/ApiTokens/1"));
        Assert.That(createdResult.Value, Is.InstanceOf<GetApiTokenDetailsResponse>());

        var createTokenResponse = createdResult.Value as GetApiTokenDetailsResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createTokenResponse, Is.Not.Null);
            Assert.That(createTokenResponse.Name, Is.EqualTo("Token"));
            Assert.That(createTokenResponse.Id, Is.EqualTo(1));
            Assert.That(createTokenResponse.Scopes, Is.EqualTo(new List<TokenScopes> { }));
            Assert.That(createTokenResponse.ExpirationDate, Is.EqualTo(new DateTimeOffset()));
            Assert.That(createTokenResponse.Token, Is.EqualTo("SuperCriticalSecret"));
        });
    }

    [Test]
    public async Task RegenerateApiToken_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<RegenerateApiTokenCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.RegenerateApiToken(1));
    }

    [Test]
    public async Task RegenerateApiToken_ReturnsTokenwithValueTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<RegenerateApiTokenCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new ApiToken
                {
                    Name = "Token",
                    Id = 1,
                    Scopes = [],
                    ExpirationDate = new DateTimeOffset(),
                    Token = "SuperCriticalSecret",
                }
            );
        var result = await _controller.RegenerateApiToken(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetApiTokenDetailsResponse>());

        var createTokenResponse = okResult.Value as GetApiTokenDetailsResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createTokenResponse, Is.Not.Null);
            Assert.That(createTokenResponse.Name, Is.EqualTo("Token"));
            Assert.That(createTokenResponse.Id, Is.EqualTo(1));
            Assert.That(createTokenResponse.Scopes, Is.EqualTo(new List<TokenScopes> { }));
            Assert.That(createTokenResponse.ExpirationDate, Is.EqualTo(new DateTimeOffset()));
            Assert.That(createTokenResponse.Token, Is.EqualTo("SuperCriticalSecret"));
        });
    }

    [Test]
    public async Task DeleteApiToken_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<DeleteApiTokenCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.DeleteApiToken(1));
    }

    [Test]
    public async Task DeleteApiToken_NoContentResponseTest()
    {
        var result = await _controller.DeleteApiToken(1);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}

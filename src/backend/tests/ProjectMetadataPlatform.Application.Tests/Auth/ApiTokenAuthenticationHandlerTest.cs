using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Tests.Auth;

[TestFixture]
public class ApiTokenAuthenticationHandlerTest
{
    private Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _optionsMock;
    private Mock<UrlEncoder> _encoderMock;
    private Mock<IApiTokenRepository> _apiTokenRepositoryMock;
    private ApiTokenAuthenticationHandler _apiTokenAuthenticationHandler;

    [SetUp]
    public void Setup()
    {
        _optionsMock = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
        _ = _optionsMock
            .Setup(x => x.Get(It.IsAny<string>()))
            .Returns(new AuthenticationSchemeOptions());
        _encoderMock = new Mock<UrlEncoder>();
        _apiTokenRepositoryMock = new Mock<IApiTokenRepository>();
        _apiTokenAuthenticationHandler = new ApiTokenAuthenticationHandler(
            _optionsMock.Object,
            _encoderMock.Object,
            _apiTokenRepositoryMock.Object
        );
    }

    [Test]
    public async Task ApiTokenAuthenticationHandler_MissingHeaderTest()
    {
        var context = new DefaultHttpContext();

        await _apiTokenAuthenticationHandler.InitializeAsync(
            new AuthenticationScheme("ApiToken", null, typeof(ApiTokenAuthenticationHandler)),
            context
        );
        var result = await _apiTokenAuthenticationHandler.AuthenticateAsync();

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Failure?.Message, Is.EqualTo("Missing Authorization Header"));
        _apiTokenRepositoryMock.Verify(m => m.GetVerifiedToken(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ApiTokenAuthenticationHandler_WrongHeaderFormatTest()
    {
        var context = new DefaultHttpContext();
        var authorizationHeader = new StringValues("Basic VGVzdFVzZXJOYW1lOlRlc3RQYXNzd29yZA==");
        context.Request.Headers.Append(HeaderNames.Authorization, authorizationHeader);
        await _apiTokenAuthenticationHandler.InitializeAsync(
            new AuthenticationScheme("ApiToken", null, typeof(ApiTokenAuthenticationHandler)),
            context
        );
        var result = await _apiTokenAuthenticationHandler.AuthenticateAsync();

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Failure?.Message, Is.EqualTo("Invalid Scheme"));
        _apiTokenRepositoryMock.Verify(m => m.GetVerifiedToken(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ApiTokenAuthenticationHandler_NoVerifiedTokenTest()
    {
        _ = _apiTokenRepositoryMock
            .Setup(m => m.GetVerifiedToken(It.IsAny<string>()))
            .ReturnsAsync((ApiToken?)null);
        var context = new DefaultHttpContext();
        var authorizationHeader = new StringValues("Bearer VGVzdFVzZXJOYW1lOlRlc3RQYXNzd29yZA==");
        context.Request.Headers.Append(HeaderNames.Authorization, authorizationHeader);
        await _apiTokenAuthenticationHandler.InitializeAsync(
            new AuthenticationScheme("ApiToken", null, typeof(ApiTokenAuthenticationHandler)),
            context
        );
        var result = await _apiTokenAuthenticationHandler.AuthenticateAsync();

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Failure?.Message, Is.EqualTo("Invalid Token"));
        _apiTokenRepositoryMock.Verify(m => m.GetVerifiedToken(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ApiTokenAuthenticationHandler_ExpiredTokenTest()
    {
        _ = _apiTokenRepositoryMock
            .Setup(m => m.GetVerifiedToken(It.IsAny<string>()))
            .ReturnsAsync(
                new ApiToken
                {
                    Name = "Token",
                    ExpirationDate = new System.DateTimeOffset(),
                    Token = "tokenHash",
                }
            );
        var context = new DefaultHttpContext();
        var authorizationHeader = new StringValues("Bearer VGVzdFVzZXJOYW1lOlRlc3RQYXNzd29yZA==");
        context.Request.Headers.Append(HeaderNames.Authorization, authorizationHeader);
        await _apiTokenAuthenticationHandler.InitializeAsync(
            new AuthenticationScheme("ApiToken", null, typeof(ApiTokenAuthenticationHandler)),
            context
        );
        var result = await _apiTokenAuthenticationHandler.AuthenticateAsync();

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Failure?.Message, Is.EqualTo("Invalid Token"));
        _apiTokenRepositoryMock.Verify(m => m.GetVerifiedToken(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ApiTokenAuthenticationHandler_SuccessfullAuthenticationTest()
    {
        _ = _apiTokenRepositoryMock
            .Setup(m => m.GetVerifiedToken(It.IsAny<string>()))
            .ReturnsAsync(
                new ApiToken
                {
                    Name = "Token",
                    ExpirationDate = DateTime.Now.AddYears(100),
                    Token = "tokenHash",
                }
            );
        var context = new DefaultHttpContext();
        var authorizationHeader = new StringValues("Bearer VGVzdFVzZXJOYW1lOlRlc3RQYXNzd29yZA==");
        context.Request.Headers.Append(HeaderNames.Authorization, authorizationHeader);
        await _apiTokenAuthenticationHandler.InitializeAsync(
            new AuthenticationScheme("ApiToken", null, typeof(ApiTokenAuthenticationHandler)),
            context
        );
        var result = await _apiTokenAuthenticationHandler.AuthenticateAsync();

        Assert.That(result.Succeeded, Is.True);
        Assert.That(
            result.Ticket?.Principal.FindFirstValue(ClaimTypes.AuthenticationMethod),
            Is.EqualTo("API Token")
        );
        Assert.That(result.Ticket?.Principal.FindFirstValue(ClaimTypes.Name), Is.EqualTo("Token"));
        _apiTokenRepositoryMock.Verify(m => m.GetVerifiedToken(It.IsAny<string>()), Times.Once);
    }
}

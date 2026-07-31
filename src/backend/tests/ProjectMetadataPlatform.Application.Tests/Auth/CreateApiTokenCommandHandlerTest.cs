using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Tests.Auth;

[TestFixture]
public class CreateApiTokenCommandHandlerTest
{
    private Mock<IApiTokenRepository> _apiTokenRepositoryMock;
    private Mock<ILogRepository> _logRepositoryMock;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;

    private CreateApiTokenCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _apiTokenRepositoryMock = new Mock<IApiTokenRepository>();
        _logRepositoryMock = new Mock<ILogRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _handler = new CreateApiTokenCommandHandler(
            _apiTokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _logRepositoryMock.Object,
            _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task CreateApiTokenCommand_SCIMTokenExistsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApiToken>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _apiTokenRepositoryMock.Setup(m => m.CheckScimTokenExists()).ReturnsAsync(true);

        var request = new CreateApiTokenCommand("Token", [TokenScopes.SCIM]);

        _ = Assert.ThrowsAsync<ScimTokenAlreadyExistsException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public async Task CreateApiTokenCommand_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApiToken>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);

        var request = new CreateApiTokenCommand("Token", [TokenScopes.SCIM]);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public async Task CreateApiTokenCommand_SuccessfulCreationTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApiToken>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _apiTokenRepositoryMock.Setup(m => m.CheckScimTokenExists()).ReturnsAsync(false);

        var request = new CreateApiTokenCommand("Token", [TokenScopes.SCIM]);

        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());
        _apiTokenRepositoryMock.Verify(m => m.StoreApiToken(It.IsAny<ApiToken>()), Times.Once);
        _authorizationServiceMock.Verify(
            a => a.CheckAccess(It.IsAny<ApiToken>(), AuthorizationConstants.Actions.CREATE, null),
            Times.Once
        );
        Assert.That(result.Name, Is.EqualTo("Token"));
        Assert.That(result.Scopes, Is.EqualTo(new List<TokenScopes> { TokenScopes.SCIM }));
        Assert.That(result.ExpirationDate, Is.GreaterThan(new DateTimeOffset()));
        _logRepositoryMock.Verify(
            m =>
                m.AddApiTokenLogForCurrentActor(
                    It.IsAny<ApiToken>(),
                    Domain.Logs.Action.ADDED_API_TOKEN,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Tests.Auth;

[TestFixture]
public class DeleteApiTokenCommandHandlerTest
{
    private Mock<IApiTokenRepository> _apiTokenRepositoryMock;
    private Mock<ILogRepository> _logRepositoryMock;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;

    private DeleteApiTokenCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _apiTokenRepositoryMock = new Mock<IApiTokenRepository>();
        _logRepositoryMock = new Mock<ILogRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _handler = new DeleteApiTokenCommandHandler(
            _apiTokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _logRepositoryMock.Object,
            _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task DeleteApiTokenCommand_SuccessfullDeletion()
    {
        var token = new ApiToken
        {
            Id = 1,
            Name = "Token",
            Token = "TokeHash",
            Scopes = new List<TokenScopes> { TokenScopes.SCIM },
        };

        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApiToken>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _apiTokenRepositoryMock
            .Setup(m => m.GetApiTokenById(It.IsAny<int>()))
            .ReturnsAsync(token);

        var request = new DeleteApiTokenCommand(1);

        await _handler.Handle(request, It.IsAny<CancellationToken>());
        _authorizationServiceMock.Verify(
            a => a.CheckAccess(token, AuthorizationConstants.Actions.DELETE, null),
            Times.Once()
        );
        _apiTokenRepositoryMock.Verify(m => m.DeleteApiToken(token), Times.Once);
        _logRepositoryMock.Verify(
            m =>
                m.AddApiTokenLogForCurrentActor(
                    token,
                    Domain.Logs.Action.REMOVED_API_TOKEN,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task DeleteApiTokenCommand_AuthorizationFailsThrows()
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

        var request = new DeleteApiTokenCommand(1);
        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

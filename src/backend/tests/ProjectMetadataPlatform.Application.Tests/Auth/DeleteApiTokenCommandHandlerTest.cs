using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Tests.Auth;

[TestFixture]
public class DeleteApiTokenCommandHandlerTest
{
    private Mock<IApiTokenRepository> _apiTokenRepositoryMock;
    private Mock<ILogRepository> _logRepositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;

    private DeleteApiTokenCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _apiTokenRepositoryMock = new Mock<IApiTokenRepository>();
        _logRepositoryMock = new Mock<ILogRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteApiTokenCommandHandler(
            _apiTokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _logRepositoryMock.Object
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
        _apiTokenRepositoryMock.Setup(m => m.GetApiTokenById(It.IsAny<int>())).ReturnsAsync(token);

        var request = new DeleteApiTokenCommand(1);

        await _handler.Handle(request, It.IsAny<CancellationToken>());
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
}

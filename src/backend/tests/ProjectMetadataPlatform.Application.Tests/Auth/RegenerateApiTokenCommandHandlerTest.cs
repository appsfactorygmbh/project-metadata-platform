using System;
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
public class RegenerateApiTokenCommandHandlerTest
{
    private Mock<IApiTokenRepository> _apiTokenRepositoryMock;
    private Mock<ILogRepository> _logRepositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;

    private RegenerateApiTokenCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _apiTokenRepositoryMock = new Mock<IApiTokenRepository>();
        _logRepositoryMock = new Mock<ILogRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new RegenerateApiTokenCommandHandler(
            _apiTokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _logRepositoryMock.Object
        );
    }

    [Test]
    public async Task RegenerateApiTokenCommand_SuccessfulRegenerationTest()
    {
        _apiTokenRepositoryMock
            .Setup(m => m.GetApiTokenById(It.IsAny<int>()))
            .ReturnsAsync(
                new ApiToken
                {
                    Name = "Token",
                    Scopes = new List<TokenScopes> { TokenScopes.SCIM },
                    Token = "TokenHash",
                    ExpirationDate = new DateTimeOffset(),
                }
            );

        var request = new RegenerateApiTokenCommand(1);

        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());
        _apiTokenRepositoryMock.Verify(m => m.UpdateApiToken(It.IsAny<ApiToken>()), Times.Once);
        Assert.That(result.Name, Is.EqualTo("Token"));
        Assert.That(result.Scopes, Is.EqualTo(new List<TokenScopes> { TokenScopes.SCIM }));
        Assert.That(result.Name, Is.Not.EqualTo("TokenHash"));
        Assert.That(result.ExpirationDate, Is.GreaterThan(new DateTimeOffset()));
        _logRepositoryMock.Verify(
            m =>
                m.AddApiTokenLogForCurrentActor(
                    It.IsAny<ApiToken>(),
                    Domain.Logs.Action.REGENERATED_API_TOKEN,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
    }
}

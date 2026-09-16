using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Tests.Auth;

[TestFixture]
public class CleanUpRefreshTokensCommandHandlerTest
{
    private CleanUpRefreshTokensCommandHandler _handler;
    private Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CleanUpRefreshTokensCommandHandler(
            _mockRefreshTokenRepo.Object,
            _unitOfWorkMock.Object,
            _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task CleanUpRefreshTokensCommand_NoTokensCallsWithEmptyList()
    {
        _mockRefreshTokenRepo.Setup(m => m.GetExpiredTokens()).ReturnsAsync([]);
        await _handler.Handle(new CleanUpRefreshTokensCommand());

        _mockRefreshTokenRepo.Verify(
            m => m.DeleteRefreshTokens(It.Is<IEnumerable<RefreshToken>>(tokens => !tokens.Any())),
            Times.Once
        );
    }

    [Test]
    public async Task CleanUpRefreshTokensCommand_ExpiredTokensCallsWithTokenList()
    {
        IEnumerable<RefreshToken> tokenList = [new RefreshToken(), new RefreshToken()];
        _mockRefreshTokenRepo.Setup(m => m.GetExpiredTokens()).ReturnsAsync(tokenList);
        await _handler.Handle(new CleanUpRefreshTokensCommand());

        _mockRefreshTokenRepo.Verify(
            m =>
                m.DeleteRefreshTokens(
                    It.Is<IEnumerable<RefreshToken>>(tokens => tokens == tokenList)
                ),
            Times.Once
        );
    }
}

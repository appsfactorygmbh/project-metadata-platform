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

namespace ProjectMetadataPlatform.Application.Tests.Auth;

[TestFixture]
public class GetApiTokenDetailsQueryHandlerTest
{
    private Mock<IApiTokenRepository> _apiTokenRepositoryMock;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private GetApiTokenDetailsQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _apiTokenRepositoryMock = new Mock<IApiTokenRepository>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _handler = new GetApiTokenDetailsQueryHandler(
            _apiTokenRepositoryMock.Object,
            _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetAllApiTokensQuery_ReturnsApiTokens()
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

        ApiToken token = new ApiToken { Name = "Token1", Token = "TokenHash1" };

        _ = _apiTokenRepositoryMock
            .Setup(m => m.GetApiTokenById(It.IsAny<int>()))
            .ReturnsAsync(token);
        var request = new GetApiTokenDetailsQuery(1);
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result.Item1, Is.EqualTo(token));
        _authorizationServiceMock.Verify(
            a => a.CheckAccess(token, AuthorizationConstants.Actions.GET, null),
            Times.Once
        );
    }

    [Test]
    public async Task GetAllApiTokensQuery_AuthorizationFailsThrowsTest()
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

        var request = new GetApiTokenDetailsQuery(1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

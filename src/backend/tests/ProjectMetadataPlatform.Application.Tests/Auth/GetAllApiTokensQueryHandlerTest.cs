using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Tests.Auth;

[TestFixture]
public class GetAllApiTokensQueryHandlerTest
{
    private Mock<IApiTokenRepository> _apiTokenRepositoryMock;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private GetAllApiTokensQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _apiTokenRepositoryMock = new Mock<IApiTokenRepository>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _handler = new GetAllApiTokensQueryHandler(
            _apiTokenRepositoryMock.Object,
            _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetAllApiTokensQuery_ReturnsQueriedApiTokens()
    {
        IList<ApiToken> tokens =
        [
            new ApiToken { Name = "Token1", Token = "TokenHash1" },
            new ApiToken { Name = "Token2", Token = "TokenHash2" },
        ];

        _ = _apiTokenRepositoryMock.Setup(m => m.GetApiTokens()).ReturnsAsync(tokens.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<ApiToken>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<ApiToken> apiTokens, Dictionary<string, string>? dict) => apiTokens
            );
        var request = new GetAllApiTokensQuery();
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(tokens));

        _authorizationServiceMock.Verify(
            a => a.TryGetPlanResourceQuery(It.IsAny<IQueryable<ApiToken>>(), null),
            Times.Once
        );
        _authorizationServiceMock.Verify(
            a =>
                a.CheckAccess(
                    It.IsAny<ApiToken>(),
                    new List<AuthorizationConstants.Actions> { AuthorizationConstants.Actions.GET },
                    null
                ),
            Times.Never
        );
    }

    [Test]
    public async Task GetAllApiTokensQuery_ReturnsFilteredApiTokens()
    {
        IList<ApiToken> tokens =
        [
            new ApiToken { Name = "Token1", Token = "TokenHash1" },
            new ApiToken { Name = "Token2", Token = "TokenHash2" },
        ];

        _ = _apiTokenRepositoryMock.Setup(m => m.GetApiTokens()).ReturnsAsync(tokens.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<ApiToken>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<ApiToken>?)null);
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApiToken>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.GET, true },
                }
            );
        var request = new GetAllApiTokensQuery();
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(tokens));
        _authorizationServiceMock.Verify(
            a => a.TryGetPlanResourceQuery(It.IsAny<IQueryable<ApiToken>>(), null),
            Times.Once
        );
        _authorizationServiceMock.Verify(
            a =>
                a.CheckAccess(
                    It.IsAny<ApiToken>(),
                    new List<AuthorizationConstants.Actions> { AuthorizationConstants.Actions.GET },
                    null
                ),
            Times.Exactly(2)
        );
    }
}

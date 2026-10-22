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
    private Mock<IPaginationHelper> _paginationHelperMock;
    private GetAllApiTokensQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _apiTokenRepositoryMock = new Mock<IApiTokenRepository>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelperMock = new Mock<IPaginationHelper>();

        _handler = new GetAllApiTokensQueryHandler(
            _apiTokenRepositoryMock.Object,
            _authorizationServiceMock.Object,
            _paginationHelperMock.Object
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

        _ = _apiTokenRepositoryMock
            .Setup(m => m.GetApiTokens(null))
            .ReturnsAsync(tokens.BuildMock());
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<ApiToken>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<ApiToken> apiTokens, int? limit) => (apiTokens, null));
        var request = new GetAllApiTokensQuery(null, null);
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result.Item1, Is.EqualTo(tokens));

        _paginationHelperMock.Verify(
            a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<ApiToken>>(), null),
            Times.Once
        );
        _authorizationServiceMock.Verify(
            a => a.CheckAccess(It.IsAny<ApiToken>(), AuthorizationConstants.Actions.GET, null),
            Times.Never
        );
    }
}

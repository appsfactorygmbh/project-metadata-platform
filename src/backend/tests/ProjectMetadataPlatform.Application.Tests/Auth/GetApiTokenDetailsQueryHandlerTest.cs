using System.Collections.Generic;
using System.Linq;
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
public class GetApiTokenDetailsQueryHandlerTest
{
    private Mock<IApiTokenRepository> _apiTokenRepositoryMock;

    private GetApiTokenDetailsQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _apiTokenRepositoryMock = new Mock<IApiTokenRepository>();

        _handler = new GetApiTokenDetailsQueryHandler(_apiTokenRepositoryMock.Object);
    }

    [Test]
    public async Task GetAllApiTokensQuery_ReturnsApiTokens()
    {
        ApiToken token = new ApiToken { Name = "Token1", Token = "TokenHash1" };

        _apiTokenRepositoryMock.Setup(m => m.GetApiTokenById(It.IsAny<int>())).ReturnsAsync(token);
        var request = new GetApiTokenDetailsQuery(1);
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(token));
    }
}


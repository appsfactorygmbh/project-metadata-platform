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
public class GetAllApiTokensQueryHandlerTest
{
    private Mock<IApiTokenRepository> _apiTokenRepositoryMock;

    private GetAllApiTokensQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _apiTokenRepositoryMock = new Mock<IApiTokenRepository>();

        _handler = new GetAllApiTokensQueryHandler(_apiTokenRepositoryMock.Object);
    }

    [Test]
    public async Task GetAllApiTokensQuery_ReturnsApiTokens()
    {
        IEnumerable<ApiToken> tokens =
        [
            new ApiToken { Name = "Token1", Token = "TokenHash1" },
            new ApiToken { Name = "Token2", Token = "TokenHash2" },
        ];
        _apiTokenRepositoryMock.Setup(m => m.GetApiTokens()).ReturnsAsync(tokens);
        var request = new GetAllApiTokensQuery();
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(tokens));
    }
}


using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Authorization;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Tests.Authorization;

[TestFixture]
public class GetPermissionsQueryHandlerTest
{
    private readonly Mock<IAuthorizationService> _authorizationService =
        new Mock<IAuthorizationService>();

    private GetPermissionsQueryHandler _getPermissionsQueryHandler;

    [SetUp]
    public void Setup()
    {
        _getPermissionsQueryHandler = new GetPermissionsQueryHandler(_authorizationService.Object);
    }

    [Test]
    public async Task GetPermissionsQueryHandler_Test()
    {
        var permissions = new Dictionary<AuthorizationConstants.Actions, FilterTree>
        {
            {
                AuthorizationConstants.Actions.CREATE,
                new FilterTree { NodeValue = "A Filter", ChildNodes = null }
            },
        };
        _authorizationService
            .Setup(s => s.GetPermissions(It.IsAny<string>()))
            .ReturnsAsync(permissions);

        var result = await _getPermissionsQueryHandler.Handle(
            new GetPermissionsQuery("A Resource")
        );
        Assert.That(result, Is.EqualTo(permissions));
        _authorizationService.Verify(s => s.GetPermissions(It.IsAny<string>()), Times.Once);
        _authorizationService.Verify(s => s.BypassAuthorization(), Times.Once);
    }
}

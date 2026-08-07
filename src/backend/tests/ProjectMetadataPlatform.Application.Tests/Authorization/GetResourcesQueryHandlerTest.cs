using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Authorization;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.Tests.Authorization;

[TestFixture]
public class GetResourcesQueryHandlerTest
{
    private readonly Mock<IAuthorizationService> _authorizationService =
        new Mock<IAuthorizationService>();

    private readonly Mock<IAuthorizationAdminService> _authorizationAdminService =
        new Mock<IAuthorizationAdminService>();
    private GetResourcesQueryHandler _getResourcesQueryHandler;

    [SetUp]
    public void Setup()
    {
        _getResourcesQueryHandler = new GetResourcesQueryHandler(
            _authorizationAdminService.Object,
            _authorizationService.Object
        );
    }

    [Test]
    public async Task GetResourcesQueryHandler_Test()
    {
        var resources = new List<string> { "Project", "User" };
        _authorizationAdminService.Setup(s => s.GetResources()).ReturnsAsync(resources);

        var result = await _getResourcesQueryHandler.Handle(new GetResourcesQuery());
        Assert.That(result, Is.EqualTo(resources));
        _authorizationAdminService.Verify(s => s.GetResources(), Times.Once);
        _authorizationService.Verify(s => s.BypassAuthorization(), Times.Once);
    }
}

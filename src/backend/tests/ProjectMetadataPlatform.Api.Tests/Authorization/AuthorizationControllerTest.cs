using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Authorization;
using ProjectMetadataPlatform.Api.Authorization.Models;
using ProjectMetadataPlatform.Application.Authorization;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Tests.Authorization;

[TestFixture]
public class AuthorizationControllerTest
{
    private AuthorizationController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new AuthorizationController(_mediator.Object);
    }

    [Test]
    public async Task GetPermissions_ReturnsPermissions_Test()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<GetPermissionsQuery, Dictionary<AuthorizationConstants.Actions, FilterTree>>(
                    It.IsAny<GetPermissionsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, FilterTree>
                {
                    {
                        AuthorizationConstants.Actions.CREATE,
                        new FilterTree { NodeValue = "A Filter", ChildNodes = null }
                    },
                }
            );
        var result = await _controller.GetPermissions("A Resource");
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetPermissionResponse>>());
        var response = (okResult.Value as IEnumerable<GetPermissionResponse>)!.ToList();
        Assert.That(response, Is.Not.Null);

        Assert.That(response, Has.Count.EqualTo(1));
        Assert.That(response[0].Action, Is.EqualTo(AuthorizationConstants.Actions.CREATE));
        Assert.That(response[0].Filter.Value, Is.EqualTo("A Filter"));
    }

    [Test]
    public async Task GetPermissions_MediatorThrowsException_Test()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<GetPermissionsQuery, Dictionary<AuthorizationConstants.Actions, FilterTree>>(
                    It.IsAny<GetPermissionsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.GetPermissions("A Resource")
        );
    }

    [Test]
    public async Task GetResources_ReturnsResources_Test()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<GetResourcesQuery, IEnumerable<string>>(
                    It.IsAny<GetResourcesQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(["Project", "User"]);
        var result = await _controller.GetResources();
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<string>>());
        var response = (okResult.Value as IEnumerable<string>)!.ToList();
        Assert.That(response, Is.Not.Null);

        Assert.That(response, Has.Count.EqualTo(2));
        Assert.That(response[0], Is.EqualTo("Project"));
        Assert.That(response[1], Is.EqualTo("User"));
    }

    [Test]
    public async Task GetResources_MediatorThrowsException_Test()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<GetResourcesQuery, IEnumerable<string>>(
                    It.IsAny<GetResourcesQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.GetResources());
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Users;

namespace ProjectMetadataPlatform.Api.Tests.Users;

[TestFixture]
public class GetAllUsersControllerTest
{
    private UsersController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new UsersController(_mediator.Object, null!);
    }

    [Test]
    public async Task Get_ReturnsAllUsers()
    {
        var users = new List<IdentityUser>
        {
            new IdentityUser { Id = "1", Email = "Hinz" },
            new IdentityUser { Id = "2", Email = "Kunz" },
        };
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await _controller.Get();

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = (okResult.Value as IEnumerable<GetUserResponse>)?.ToList();

        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response, Has.Count.EqualTo(2));
            Assert.That(response.ElementAt(0).Id, Is.EqualTo("1"));
            Assert.That(response.ElementAt(0).Email, Is.EqualTo("Hinz"));
            Assert.That(response.ElementAt(1).Id, Is.EqualTo("2"));
            Assert.That(response.ElementAt(1).Email, Is.EqualTo("Kunz"));
        });
    }

    [Test]
    public async Task Get_ReturnsEmptyList()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IdentityUser>());

        var result = await _controller.Get();

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as IEnumerable<GetUserResponse>;
        Assert.That(response, Is.Not.Null.And.Empty);
    }

    [Test]
    public void Get_ReturnsMediatorException()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Get());
    }
}

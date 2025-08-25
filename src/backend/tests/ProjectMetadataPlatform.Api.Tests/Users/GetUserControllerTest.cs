using System.IO;
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
using ProjectMetadataPlatform.Domain.Errors.UserException;

namespace ProjectMetadataPlatform.Api.Tests.Users;

[TestFixture]
public class GetUserControllerTest
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
    public async Task Get_ReturnsUser()
    {
        var user = new IdentityUser { Id = "1", Email = "Hinz" };
        _mediator
            .Setup(m => m.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _controller.GetUserById("1");

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as GetUserResponse;
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo("1"));
            Assert.That(response.Email, Is.EqualTo("Hinz"));
        });
    }

    [Test]
    public void GetUserById_NonexistentUser_Test()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new UserNotFoundException("1"));
        Assert.ThrowsAsync<UserNotFoundException>(() => _controller.GetUserById("1"));
    }

    [Test]
    public void MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.GetUserById("1"));
    }
}

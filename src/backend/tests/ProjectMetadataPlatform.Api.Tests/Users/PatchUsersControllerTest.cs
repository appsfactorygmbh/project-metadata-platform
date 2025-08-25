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

public class PatchUsersControllerTest
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
    public async Task PatchUser_Test()
    {
        var user = new IdentityUser
        {
            Id = "42",
            Email = "dr@core.fr",
            PasswordHash = "someHash",
        };

        _mediator
            .Setup(m => m.Send(It.IsAny<PatchUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var request = new PatchUserRequest(null, "Dr. Peacock");

        var result = await _controller.Patch("42", request);
        var okResult = result.Result as OkObjectResult;
        var resultValue = okResult?.Value as GetUserResponse;

        Assert.That(resultValue, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultValue.Email, Is.EqualTo("dr@core.fr"));
            Assert.That(resultValue.Id, Is.EqualTo("42"));
        });
    }

    [Test]
    public void PatchUser_NotFound_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<PatchUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserNotFoundException("Dr. Dre"));
        var request = new PatchUserRequest(null, "Dr. Dre");
        Assert.ThrowsAsync<UserNotFoundException>(() => _controller.Patch("Dr. Dre", request));
    }

    [Test]
    public void PatchUser_InvalidPassword_Test()
    {
        var request = new PatchUserRequest(null, "The Smiths");
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<PatchUserCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new UserInvalidPasswordFormatException(IdentityResult.Failed()));
        Assert.ThrowsAsync<UserInvalidPasswordFormatException>(() =>
            _controller.Patch("13", request)
        );
    }
}

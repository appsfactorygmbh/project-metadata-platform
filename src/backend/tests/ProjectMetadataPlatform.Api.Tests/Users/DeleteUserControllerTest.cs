using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Errors.UserException;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Api.Tests.Users;

[TestFixture]
public class DeleteUserControllerTest
{
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new UsersController(_mediator.Object, null!);
    }

    private UsersController _controller;
    private Mock<IMediator> _mediator;

    [Test]
    public async Task DeleteUser_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "2",
            Id = "1",
            Email = "John",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _mediator
            .Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var result = await _controller.Delete("1");
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mediator.Verify(mediator =>
            mediator.Send(
                It.Is<DeleteUserCommand>(command => command.EmployeeId == "1"),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public void DeleteUser_NotFound_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserNotFoundException("Mike"));
        Assert.ThrowsAsync<UserNotFoundException>(() => _controller.Delete("Mike"));
    }

    [Test]
    public void DeleteUser_InternalError_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserNotFoundException("Mike"));
        Assert.ThrowsAsync<UserNotFoundException>(() => _controller.Delete("Mike"));
    }

    [Test]
    public void DeleteUser_UserSelfDeletionAttempt_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserCantDeleteThemselfException());

        Assert.ThrowsAsync<UserCantDeleteThemselfException>(() => _controller.Delete("1"));
    }
}

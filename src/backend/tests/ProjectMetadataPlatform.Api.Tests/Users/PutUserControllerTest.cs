using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Users;

namespace ProjectMetadataPlatform.Api.Tests.Users;

[TestFixture]
public class PutUserControllerTest
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
    public async Task CreateUser_Test()
    {
        //prepare
        _mediator
            .Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("1");
        var request = new CreateUserRequest("Example Email", "Example Password");
        var result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateUserResponse>());

        var userResponse = createdResult.Value as CreateUserResponse;
        Assert.That(userResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(userResponse.UserId, Is.EqualTo("1"));

            Assert.That(createdResult.Location, Is.EqualTo("/Users/1"));
        });
        _mediator.Verify(mediator =>
            mediator.Send(
                It.Is<CreateUserCommand>(command =>
                    command.Email == "Example Email" && command.Password == "Example Password"
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public void CreateUser_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidOperationException());
        var request = new CreateUserRequest("Example Email", "Example Password");

        Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Put(request));
    }
}

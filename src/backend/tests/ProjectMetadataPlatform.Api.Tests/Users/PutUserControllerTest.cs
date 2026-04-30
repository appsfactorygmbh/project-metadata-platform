using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

namespace ProjectMetadataPlatform.Api.Tests.Users;

[TestFixture]
public class PutUserControllerTest
{
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _context = new Mock<IHttpContextAccessor>();
        _controller = new UsersController(_mediator.Object, _context.Object);
    }

    private UsersController _controller;
    private Mock<IMediator> _mediator;

    private Mock<IHttpContextAccessor> _context;

    [Test]
    public async Task CreateUser_Test()
    {
        //prepare
        _mediator
            .Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new Domain.Users.ApplicationUser
                {
                    EmployeeId = "Id",
                    Id = "1",
                    Email = "dr@core.fr",
                    PasswordHash = "someHash",
                    IsActive = true,
                    IsScimProvisioned = false,
                }
            );
        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.AuthenticationMethod, "API Token") },
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed
        var httpContext = new DefaultHttpContext { User = contextUser };
        _context.Setup(accessor => accessor.HttpContext).Returns(httpContext);
        var request = new PmpScimUser
        {
            Id = "Id",
            ExternalId = "Id",
            UserName = "dr@core.fr",
            Password = "SomePassword",
            Active = true,
        };
        var result = await _controller.Post(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<PmpScimUser>());

        var userResponse = createdResult.Value as PmpScimUser;
        Assert.That(userResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(userResponse.Id, Is.EqualTo("Id"));
            Assert.That(userResponse.ExternalId, Is.EqualTo("Id"));
            Assert.That(userResponse.UserName, Is.EqualTo("dr@core.fr"));
            Assert.That(userResponse.Password, Is.Null);
            Assert.That(userResponse.Active, Is.EqualTo(true));
            Assert.That(createdResult.Location, Is.EqualTo("/Users/Id"));
        });
        _mediator.Verify(mediator =>
            mediator.Send(
                It.Is<CreateUserCommand>(command =>
                    command.Email == "dr@core.fr"
                    && command.Password == "SomePassword"
                    && command.EmployeeId == "Id"
                    && command.IsActive == true
                    && command.IsScimProvisioned == true
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
        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.AuthenticationMethod, "JWT Token") },
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed
        var httpContext = new DefaultHttpContext { User = contextUser };
        _context.Setup(accessor => accessor.HttpContext).Returns(httpContext);

        var request = new PmpScimUser
        {
            Id = "Id",
            ExternalId = "Id",
            UserName = "dr@core.fr",
            Password = "SomePassword",
            Active = true,
        };

        Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Post(request));
    }

    [Test]
    public void CreateUser_ThrowsUnknownAuthExceptionTest()
    {
        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.AuthenticationMethod, " Token") },
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed
        var httpContext = new DefaultHttpContext { User = contextUser };
        _context.Setup(accessor => accessor.HttpContext).Returns(httpContext);

        var request = new PmpScimUser
        {
            Id = "Id",
            ExternalId = "Id",
            UserName = "dr@core.fr",
            Password = "SomePassword",
            Active = true,
        };

        Assert.ThrowsAsync<UnknownAuthentificationMethodException>(() => _controller.Post(request));
    }
}

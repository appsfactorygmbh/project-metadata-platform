using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Users;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.Errors.UserException;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Api.Tests.Users;

public class PatchUsersControllerTest
{
    private UsersController _controller;
    private Mock<IMediator> _mediator;

    private Mock<IHttpContextAccessor> _context;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _context = new Mock<IHttpContextAccessor>();
        _controller = new UsersController(_mediator.Object, _context.Object);
    }

    [Test]
    public async Task PatchUser_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "Id",
            Id = "42",
            Email = "dr@core.fr",
            PasswordHash = "someHash",
            IsActive = true,
            IsScimProvisioned = false,
        };

        _mediator
            .Setup(m => m.Send(It.IsAny<PatchUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.AuthenticationMethod, "JWT Token") },
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed
        var httpContext = new DefaultHttpContext { User = contextUser };
        _context.Setup(accessor => accessor.HttpContext).Returns(httpContext);

        var request = new PatchUserRequest { };

        var result = await _controller.Patch("42", request);
        var okResult = result.Result as OkObjectResult;
        var resultValue = okResult?.Value as PmpScimUser;

        Assert.That(resultValue, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(resultValue.UserName, Is.EqualTo("dr@core.fr"));
            Assert.That(resultValue.Id, Is.EqualTo("Id"));
        });
    }

    [Test]
    public void PatchUser_NotFound_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<PatchUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserNotFoundException("Dr. Dre"));

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.AuthenticationMethod, "JWT Token") },
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed
        var httpContext = new DefaultHttpContext { User = contextUser };
        _context.Setup(accessor => accessor.HttpContext).Returns(httpContext);

        var request = new PatchUserRequest();
        Assert.ThrowsAsync<UserNotFoundException>(() => _controller.Patch("Dr. Dre", request));
    }

    [Test]
    public void PatchUser_InvalidPassword_Test()
    {
        var request = new PatchUserRequest
        {
            Operations = new System.Collections.Generic.List<PatchUserRequest.OperationRecord>
            {
                new PatchUserRequest.OperationRecord
                {
                    Op = PatchOperations.Add,
                    Path = "Password",
                    Value = "1234",
                },
            },
        };
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<PatchUserCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new UserInvalidPasswordFormatException(IdentityResult.Failed()));

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.AuthenticationMethod, "JWT Token") },
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed
        var httpContext = new DefaultHttpContext { User = contextUser };
        _context.Setup(accessor => accessor.HttpContext).Returns(httpContext);

        Assert.ThrowsAsync<UserInvalidPasswordFormatException>(() =>
            _controller.Patch("13", request)
        );
    }

    [Test]
    public void PatchUser_UnknownAuthMethod_Test()
    {
        var request = new PatchUserRequest
        {
            Operations = new System.Collections.Generic.List<PatchUserRequest.OperationRecord>
            {
                new PatchUserRequest.OperationRecord
                {
                    Op = PatchOperations.Add,
                    Path = "Password",
                    Value = "1234",
                },
            },
        };

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.AuthenticationMethod, "JWB Token") },
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed
        var httpContext = new DefaultHttpContext { User = contextUser };
        _context.Setup(accessor => accessor.HttpContext).Returns(httpContext);

        Assert.ThrowsAsync<UnknownAuthentificationMethodException>(() =>
            _controller.Patch("13", request)
        );
    }
}

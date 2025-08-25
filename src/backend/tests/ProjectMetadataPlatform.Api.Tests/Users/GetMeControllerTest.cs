using System;
using System.Security.Claims;
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
using ProjectMetadataPlatform.Domain.Errors.UserException;

namespace ProjectMetadataPlatform.Api.Tests.Users;

public class GetMeControllerTest
{
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
    }

    [Test]
    public async Task getMe_Test()
    {
        var user = new IdentityUser { Id = "42", Email = "moonstealer@gruhq.com" };

        _mediator
            .Setup(m =>
                m.Send(
                    It.IsAny<GetUserByEmailQuery>(),
                    It.IsAny<System.Threading.CancellationToken>()
                )
            )
            .ReturnsAsync(user);
        var controller = new UsersController(
            _mediator.Object,
            MockHttpContextAccessor("moonstealer@gruhq.com")
        );

        var result = await controller.GetMe();
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as GetUserResponse;
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo("42"));
            Assert.That(response.Email, Is.EqualTo("moonstealer@gruhq.com"));
        });
    }

    [Test]
    public void getMe_Test_NotFound()
    {
        _mediator
            .Setup(m =>
                m.Send(
                    It.IsAny<GetUserByEmailQuery>(),
                    It.IsAny<System.Threading.CancellationToken>()
                )
            )
            .ThrowsAsync(new UserNotFoundException("Dr. Dre"));
        var controller = new UsersController(_mediator.Object, MockHttpContextAccessor("Dr. Dre"));
        Assert.ThrowsAsync<UserNotFoundException>(() => controller.GetMe());
    }

    [Test]
    public void getMe_Test_InternalError()
    {
        _mediator
            .Setup(m =>
                m.Send(
                    It.IsAny<GetUserByEmailQuery>(),
                    It.IsAny<System.Threading.CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidOperationException("Internal error"));
        var controller = new UsersController(
            _mediator.Object,
            MockHttpContextAccessor("Dr. Nefario")
        );

        Assert.ThrowsAsync<InvalidOperationException>(() => controller.GetMe());
    }

    [Test]
    public void getMe_Test_Unauthorized()
    {
        _mediator
            .Setup(m =>
                m.Send(
                    It.IsAny<GetUserByEmailQuery>(),
                    It.IsAny<System.Threading.CancellationToken>()
                )
            )
            .ThrowsAsync(new UserUnauthorizedException());
        var controller = new UsersController(_mediator.Object, MockHttpContextAccessor(null));

        Assert.ThrowsAsync<UserUnauthorizedException>(() => controller.GetMe());
    }

    private static HttpContextAccessor MockHttpContextAccessor(string? email)
    {
        if (email == null)
        {
            return new HttpContextAccessor { HttpContext = null };
        }
        var claims = new System.Collections.Generic.List<Claim> { new(ClaimTypes.Email, email) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        return new HttpContextAccessor { HttpContext = httpContext };
    }
}

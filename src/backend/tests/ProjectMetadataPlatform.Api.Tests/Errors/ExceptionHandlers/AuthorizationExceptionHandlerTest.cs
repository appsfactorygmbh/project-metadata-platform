using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Api.Tests.Errors.ExceptionHandlers;

public class AuthorizationExceptionHandlerTest
{
    private AuthorizationExceptionHandler _authorizationExceptionHandler;

    [SetUp]
    public void SetUp()
    {
        _authorizationExceptionHandler = new AuthorizationExceptionHandler();
    }

    [Test]
    public void Handle_UnauthorizedException_ReturnsUnauthorized()
    {
        var mockException = new Mock<UnauthorizedException>();
        var result = _authorizationExceptionHandler.Handle(mockException.Object);

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());

        var statusCodeResult = result as UnauthorizedResult;

        Assert.That(statusCodeResult?.StatusCode, Is.EqualTo(401));
    }
}

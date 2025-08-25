using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Errors.UserException;
using RouteData = Microsoft.AspNetCore.Routing.RouteData;

namespace ProjectMetadataPlatform.Api.Tests.Errors;

public class ExceptionFilterTest
{
    private ExceptionFilter _filter;
    private Mock<IExceptionHandler<PmpException>> _basicExceptionHandler;
    private Mock<IExceptionHandler<LogException>> _logExceptionHandler;
    private Mock<IExceptionHandler<ProjectException>> _projectExceptionHandler;
    private Mock<IExceptionHandler<UserException>> _userExceptionHandler;
    private Mock<IExceptionHandler<TeamException>> _teamExceptionHandler;
    private Mock<IExceptionHandler<PluginException>> _pluginsExceptionHandler;
    private Mock<IExceptionHandler<AuthException>> _authExceptionHandler;
    private Mock<ExceptionContext> _context;

    [SetUp]
    public void SetUp()
    {
        _basicExceptionHandler = new Mock<IExceptionHandler<PmpException>>();
        _projectExceptionHandler = new Mock<IExceptionHandler<ProjectException>>();
        _pluginsExceptionHandler = new Mock<IExceptionHandler<PluginException>>();
        _logExceptionHandler = new Mock<IExceptionHandler<LogException>>();
        _authExceptionHandler = new Mock<IExceptionHandler<AuthException>>();
        _userExceptionHandler = new Mock<IExceptionHandler<UserException>>();
        _teamExceptionHandler = new();
        _context = SetupExceptionContext();
        _filter = new ExceptionFilter(
            basicExceptionHandler: _basicExceptionHandler.Object,
            projectExceptionHandler: _projectExceptionHandler.Object,
            logExceptionHandler: _logExceptionHandler.Object,
            pluginExceptionHandler: _pluginsExceptionHandler.Object,
            authExceptionHandler: _authExceptionHandler.Object,
            userExceptionHandler: _userExceptionHandler.Object,
            teamExceptionHandler: _teamExceptionHandler.Object
        );
    }

    private static Mock<ExceptionContext> SetupExceptionContext()
    {
        var mockHttpContext = new Mock<HttpContext>();
        var mockRouteData = new RouteData();
        var mockActionDescriptor = new ActionDescriptor();
        var mockActionContext = new Mock<ActionContext>(
            mockHttpContext.Object,
            mockRouteData,
            mockActionDescriptor
        );
        return new Mock<ExceptionContext>(mockActionContext.Object, new List<IFilterMetadata>());
    }

    [Test]
    public void CallsBasicExceptionHandlerForBasicException_Test()
    {
        var mockException = new Mock<EntityNotFoundException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        var result = new StatusCodeResult(404);
        _basicExceptionHandler.Setup(h => h.Handle(It.IsAny<PmpException>())).Returns(result);

        _context
            .SetupSet(c => c.Result = It.IsAny<IActionResult>())
            .Callback(
                (IActionResult r) =>
                {
                    Assert.That(r, Is.EqualTo(result));
                }
            );

        _filter.OnException(_context.Object);

        _basicExceptionHandler.Verify(h => h.Handle(It.IsAny<PmpException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void CallsPluginExceptionHandlerForPluginException_Test()
    {
        var mockException = new Mock<PluginException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        var result = new StatusCodeResult(500);
        _pluginsExceptionHandler.Setup(h => h.Handle(It.IsAny<PluginException>())).Returns(result);

        _context
            .SetupSet(c => c.Result = It.IsAny<IActionResult>())
            .Callback(
                (IActionResult r) =>
                {
                    Assert.That(r, Is.EqualTo(result));
                }
            );

        _filter.OnException(_context.Object);

        _pluginsExceptionHandler.Verify(h => h.Handle(It.IsAny<PluginException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void CallsProjectsExceptionHandlerForProjectException_Test()
    {
        var mockException = new Mock<ProjectException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        var result = new StatusCodeResult(500);
        _projectExceptionHandler.Setup(h => h.Handle(It.IsAny<ProjectException>())).Returns(result);

        _context
            .SetupSet(c => c.Result = It.IsAny<IActionResult>())
            .Callback(
                (IActionResult r) =>
                {
                    Assert.That(r, Is.EqualTo(result));
                }
            );

        _filter.OnException(_context.Object);

        _projectExceptionHandler.Verify(h => h.Handle(It.IsAny<ProjectException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void CallsAuthExceptionHandlerForProjectException_Test()
    {
        var mockException = new Mock<AuthException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        var result = new StatusCodeResult(500);
        _authExceptionHandler.Setup(h => h.Handle(It.IsAny<AuthException>())).Returns(result);

        _context
            .SetupSet(c => c.Result = It.IsAny<IActionResult>())
            .Callback(
                (IActionResult r) =>
                {
                    Assert.That(r, Is.EqualTo(result));
                }
            );

        _filter.OnException(_context.Object);

        _authExceptionHandler.Verify(h => h.Handle(It.IsAny<AuthException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void HandlesUnknownException_Test()
    {
        var mockException = new Mock<Exception>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        _context
            .SetupSet(c => c.Result = It.IsAny<IActionResult>())
            .Callback(
                (IActionResult r) =>
                {
                    Assert.That(r, Is.InstanceOf<ObjectResult>());
                    var objectResult = (ObjectResult)r;
                    Assert.Multiple(() =>
                    {
                        Assert.That(
                            objectResult.StatusCode,
                            Is.EqualTo(StatusCodes.Status500InternalServerError)
                        );
                        Assert.That(
                            (objectResult.Value as ErrorResponse)?.Message,
                            Is.EqualTo("An unknown error occurred.")
                        );
                    });
                }
            );

        _filter.OnException(_context.Object);

        _basicExceptionHandler.Verify(h => h.Handle(It.IsAny<PmpException>()), Times.Never);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void CallLogsExceptionHandlerForLogException_Test()
    {
        var mockException = new Mock<LogException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        var result = new StatusCodeResult(500);
        _logExceptionHandler.Setup(h => h.Handle(It.IsAny<LogException>())).Returns(result);

        _context
            .SetupSet(c => c.Result = It.IsAny<IActionResult>())
            .Callback(
                (IActionResult r) =>
                {
                    Assert.That(r, Is.EqualTo(result));
                }
            );

        _filter.OnException(_context.Object);

        _logExceptionHandler.Verify(h => h.Handle(It.IsAny<LogException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void HandlesNullReturnOfExceptionHandler_Test()
    {
        var mockException = new Mock<ProjectException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        _context
            .SetupSet(c => c.Result = It.IsAny<IActionResult>())
            .Callback(
                (IActionResult r) =>
                {
                    Assert.That(r, Is.InstanceOf<ObjectResult>());
                    var objectResult = (ObjectResult)r;
                    Assert.Multiple(() =>
                    {
                        Assert.That(
                            objectResult.StatusCode,
                            Is.EqualTo(StatusCodes.Status500InternalServerError)
                        );
                        Assert.That(
                            (objectResult.Value as ErrorResponse)?.Message,
                            Is.EqualTo("An unknown error occurred.")
                        );
                    });
                }
            );
        _projectExceptionHandler
            .Setup(h => h.Handle(It.IsAny<ProjectException>()))
            .Returns((IActionResult?)null);

        _filter.OnException(_context.Object);

        _projectExceptionHandler.Verify(h => h.Handle(It.IsAny<ProjectException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void CallLogsExceptionHandlerForProjectException_Test()
    {
        var mockException = new Mock<LogException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        var result = new StatusCodeResult(500);
        _logExceptionHandler.Setup(h => h.Handle(It.IsAny<LogException>())).Returns(result);

        _context
            .SetupSet(c => c.Result = It.IsAny<IActionResult>())
            .Callback(
                (IActionResult r) =>
                {
                    Assert.That(r, Is.EqualTo(result));
                }
            );

        _filter.OnException(_context.Object);

        _logExceptionHandler.Verify(h => h.Handle(It.IsAny<LogException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }
}

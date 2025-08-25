using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Api.Tests.Errors.ExceptionHandlers;

public class PluginExceptionHandlerTest
{
    private PluginsExceptionHandler _pluginExceptionHandler;

    [SetUp]
    public void Setup()
    {
        _pluginExceptionHandler = new PluginsExceptionHandler();
    }

    [Test]
    public void Handle_PluginNotArchivedException_ReturnsBadRequest()
    {
        var plugin = new Plugin
        {
            Id = 1,
            PluginName = "Test Plugin",
            IsArchived = false,
            BaseUrl = "https://test.com",
            ProjectPlugins = null,
        };

        var mockException = new Mock<PluginNotArchivedException>(plugin);

        var result = _pluginExceptionHandler.Handle(mockException.Object);
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var statusCodeResult = result as BadRequestObjectResult;
        Assert.That(statusCodeResult?.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public void Handle_UnknownException_ReturnsInternalServerError()
    {
        var mockException = new Mock<PluginException>("Test Message");

        var result = _pluginExceptionHandler.Handle(mockException.Object);
        Assert.That(result, Is.Null);
    }
}

using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Api.Tests.Errors.ExceptionHandlers;

public class LogsExceptionHandlerTest
{
    private LogsExceptionHandler _logsExceptionHandler;

    [SetUp]
    public void SetUp()
    {
        _logsExceptionHandler = new LogsExceptionHandler();
    }

    [TestCase(Action.ADDED_PROJECT, "Project")]
    [TestCase(Action.ADDED_PROJECT_PLUGIN, "Project")]
    [TestCase(Action.UPDATED_PROJECT, "Project")]
    [TestCase(Action.UPDATED_PROJECT_PLUGIN, "Project")]
    [TestCase(Action.REMOVED_PROJECT_PLUGIN, "Project")]
    [TestCase(Action.ARCHIVED_PROJECT, "Project")]
    [TestCase(Action.UNARCHIVED_PROJECT, "Project")]
    [TestCase(Action.REMOVED_PROJECT, "Project")]
    [TestCase(Action.ADDED_GLOBAL_PLUGIN, "Plugin")]
    [TestCase(Action.UPDATED_GLOBAL_PLUGIN, "Plugin")]
    [TestCase(Action.ARCHIVED_GLOBAL_PLUGIN, "Plugin")]
    [TestCase(Action.UNARCHIVED_GLOBAL_PLUGIN, "Plugin")]
    [TestCase(Action.REMOVED_GLOBAL_PLUGIN, "Plugin")]
    [TestCase(Action.ADDED_USER, "IdentityUser")]
    [TestCase(Action.UPDATED_USER, "IdentityUser")]
    [TestCase(Action.REMOVED_USER, "IdentityUser")]
    public void Handle_LogsNotSupportedAction_ReturnsInternalServerError(
        Action action,
        string logType
    )
    {
        var exception = new LogActionNotSupportedException(action, logType);

        var result = _logsExceptionHandler.Handle(exception);

        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var statusCodeResult = (ObjectResult)result;
        Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));
    }
}

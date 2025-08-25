using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;
using ProjectMetadataPlatform.Domain.Errors;
using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Api.Tests.Errors.ExceptionHandlers;

public class BasicExceptionHandlerTest
{
    private BasicExceptionHandler _basicExceptionHandler;

    [SetUp]
    public void SetUp()
    {
        _basicExceptionHandler = new BasicExceptionHandler();
    }

    [Test]
    public void Handle_EntityNotFoundException_ReturnsNotFound()
    {
        var exception = new EntityNotFoundException("Entity not found");

        var result = _basicExceptionHandler.Handle(exception);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = (NotFoundObjectResult)result;
        Assert.Multiple(() =>
        {
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(
                (notFoundResult?.Value as ErrorResponse)?.Message,
                Is.EqualTo(exception.Message)
            );
        });
    }

    [Test]
    public void Handle_EntityAlreadyExistsException_ReturnsConflict()
    {
        var exception = new EntityAlreadyExistsException("Entity already exists");

        var result = _basicExceptionHandler.Handle(exception);

        Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        var conflictResult = (ConflictObjectResult)result;
        Assert.Multiple(() =>
        {
            Assert.That(conflictResult, Is.Not.Null);
            Assert.That(
                (conflictResult?.Value as ErrorResponse)?.Message,
                Is.EqualTo(exception.Message)
            );
        });
    }

    [Test]
    public void Handle_DatabaseException_ReturnsInternalServerError()
    {
        var exception = new DatabaseException(new Exception());

        var result = _basicExceptionHandler.Handle(exception);

        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var statusCodeResult = (ObjectResult)result;
        Assert.Multiple(() =>
        {
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult?.StatusCode, Is.EqualTo(502));
        });
    }

    [Test]
    public void Handle_UnknownException_ReturnsNull()
    {
        var mockException = new Mock<PmpException>("Some message");

        var result = _basicExceptionHandler.Handle(mockException.Object);

        Assert.That(result, Is.Null);
    }
}

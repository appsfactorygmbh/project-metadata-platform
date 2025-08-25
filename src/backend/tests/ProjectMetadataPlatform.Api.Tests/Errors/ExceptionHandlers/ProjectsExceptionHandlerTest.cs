using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Tests.Errors.ExceptionHandlers;

public class ProjectsExceptionHandlerTest
{
    private ProjectsExceptionHandler _projectsExceptionHandler;

    [SetUp]
    public void SetUp()
    {
        _projectsExceptionHandler = new ProjectsExceptionHandler();
    }

    [Test]
    public void Handle_ProjectNotArchivedException_ReturnsBadRequest()
    {
        var project = new Project
        {
            Id = 400,
            ProjectName = "Test Project",
            Slug = "test_project",
            ClientName = "Test Client",
        };

        var mockException = new Mock<ProjectNotArchivedException>(project);

        var result = _projectsExceptionHandler.Handle(mockException.Object);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var statusCodeResult = (BadRequestObjectResult)result;
        Assert.That(statusCodeResult, Is.Not.Null);
        Assert.That(statusCodeResult.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public void Handle_UnknownException_ReturnsNull()
    {
        var mockException = new Mock<ProjectException>("Some message");

        var result = _projectsExceptionHandler.Handle(mockException.Object);

        Assert.That(result, Is.Null);
    }
}

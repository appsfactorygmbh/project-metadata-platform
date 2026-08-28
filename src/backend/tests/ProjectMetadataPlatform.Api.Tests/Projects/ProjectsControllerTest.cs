using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Common.Models;
using ProjectMetadataPlatform.Api.Projects;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Tests.Projects;

[TestFixture]
public class ProjectsControllerTest
{
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new ProjectsController(_mediator.Object);
    }

    private ProjectsController _controller;
    private Mock<IMediator> _mediator;

    [Test]
    public async Task GetAllProjects_EmptyResponseList_Test()
    {
        // prepare
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllProjectsQuery,
                    (IEnumerable<Project>, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(([], []));

        // act
        var result = await _controller.Get(null, null);

        // assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetProjectResponse>>());

        var getProjectsResponseArray = (
            okResult.Value as GetListResponse<GetProjectResponse>
        )?.Resources.ToArray();
        Assert.That(getProjectsResponseArray, Is.Not.Null);

        Assert.That(getProjectsResponseArray, Has.Length.EqualTo(0));
    }

    [Test]
    public async Task GetAllProjectsTest()
    {
        // prepare
        var projectsResponseContent = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "Nasa",

                Company = new() { CompanyName = "Geostorm" },
                CompanyId = 1,
                IsmsLevel = SecurityLevel.VERY_HIGH,
            },
        };
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllProjectsQuery,
                    (IEnumerable<Project>, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((projectsResponseContent, []));

        // act
        var result = await _controller.Get(null, null);

        // assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetProjectResponse>>());

        var getProjectsResponseArray = (
            okResult.Value as GetListResponse<GetProjectResponse>
        )?.Resources.ToArray();
        Assert.That(getProjectsResponseArray, Is.Not.Null);

        Assert.That(getProjectsResponseArray, Has.Length.EqualTo(1));

        var project = getProjectsResponseArray.First();
        Assert.Multiple(() =>
        {
            Assert.That(project.Id, Is.EqualTo(1));
            Assert.That(project.Slug, Is.EqualTo("regen"));
            Assert.That(project.ProjectName, Is.EqualTo("Regen"));
            Assert.That(project.ClientName, Is.EqualTo("Nasa"));
            Assert.That(project.Company.CompanyName, Is.EqualTo("Geostorm"));
            Assert.That(project.IsmsLevel, Is.EqualTo(SecurityLevel.VERY_HIGH));
        });
    }

    [Test]
    public async Task GetProjectBySearchControllerTest()
    {
        // prepare
        var projectsResponseContent = new List<Project>
        {
            new()
            {
                Id = 0,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "Nasa",
                Company = new() { CompanyName = "NothingButTheBest GmbH" },
                CompanyId = 2,
                IsmsLevel = SecurityLevel.HIGH,
            },
        };

        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllProjectsQuery,
                    (IEnumerable<Project>, IEnumerable<AuthorizationConstants.Actions>)
                >(It.Is<GetAllProjectsQuery>(x => x.Search == "R"), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((projectsResponseContent, []));

        // act
        var result = await _controller.Get(null, "R");

        // assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetProjectResponse>>());

        var getProjectsResponseArray = (
            okResult.Value as GetListResponse<GetProjectResponse>
        )?.Resources.ToArray();
        Assert.That(getProjectsResponseArray, Is.Not.Null);

        Assert.That(getProjectsResponseArray, Has.Length.EqualTo(1));

        var project = getProjectsResponseArray.First();
        Assert.Multiple(() =>
        {
            Assert.That(project.ProjectName, Is.EqualTo("Regen"));
            Assert.That(project.Slug, Is.EqualTo("regen"));
            Assert.That(project.ClientName, Is.EqualTo("Nasa"));
            Assert.That(project.Company.CompanyName, Is.EqualTo("NothingButTheBest GmbH"));
            Assert.That(project.IsmsLevel, Is.EqualTo(SecurityLevel.HIGH));
        });
    }

    [Test]
    public void GetAllProjects_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<
                    GetAllProjectsQuery,
                    (IEnumerable<Project>, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get(null, "search"));
    }

    [Test]
    public async Task GetProjectByFiltersAndSearchTest()
    {
        const string search = "Hea";
        var filters = new ProjectFilterRequest(
            "Heather",
            "Metatron",
            new List<string> { "666", "777" },
            new List<string> { "42", "43" },
            true,
            true,
            new List<string> { "Optimus Prime" },
            SecurityLevel.HIGH
        );

        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllProjectsQuery,
                    (IEnumerable<Project>, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetAllProjectsQuery>(), CancellationToken.None)
            )
            .ReturnsAsync(
                (
                    new List<Project>
                    {
                        new()
                        {
                            Id = 1,
                            ProjectName = "Heather",
                            Slug = "heather",
                            ClientName = "Metatron",
                            IsArchived = true,
                            IsEoC = true,
                            CompanyId = 1,
                            Company = new() { CompanyName = "Optimus Prime" },
                            IsmsLevel = SecurityLevel.HIGH,
                        },
                    },
                    []
                )
            );

        var result = await _controller.Get(filters, search);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        var response = (okResult.Value as GetListResponse<GetProjectResponse>)?.Resources.ToList();
        Assert.That(response, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Has.Count.EqualTo(1));
            Assert.That(response.ToArray()[0].Id, Is.EqualTo(1));
            Assert.That(response.ToArray()[0].ProjectName, Is.EqualTo("Heather"));
            Assert.That(response.ToArray()[0].Slug, Is.EqualTo("heather"));
            Assert.That(response.ToArray()[0].ClientName, Is.EqualTo("Metatron"));
            Assert.That(response.ToArray()[0].IsArchived, Is.EqualTo(true));
            Assert.That(response.ToArray()[0].IsEoC, Is.EqualTo(true));
            Assert.That(response.ToArray()[0].Company.CompanyName, Is.EqualTo("Optimus Prime"));
            Assert.That(response.ToArray()[0].IsmsLevel, Is.EqualTo(SecurityLevel.HIGH));
        });
    }

    [Test]
    public async Task GetProjectByFiltersAndSearchTest_NoMatch()
    {
        var search = "Hea";
        var filters = new ProjectFilterRequest(
            "Heather",
            "Gilgamesch",
            new List<string> { "666", "777" },
            new List<string> { "42", "43" },
            false,
            false,
            new List<string> { "Minas Tirith" },
            SecurityLevel.NORMAL
        );

        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllProjectsQuery,
                    (IEnumerable<Project>, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetAllProjectsQuery>(), CancellationToken.None)
            )
            .ReturnsAsync(([], []));

        var result = await _controller.Get(filters, search);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

        var response = (okResult.Value as GetListResponse<GetProjectResponse>)?.Resources.ToArray();
        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.Empty);
        });
    }

    [Test]
    public async Task DeleteProject_ReturnsOk()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Heather",
            Slug = "heather",
            ClientName = "Metatron",
            IsArchived = true,
            CompanyId = 1,
        };

        _ = _mediator
            .Setup(m =>
                m.Send<DeleteProjectCommand, Project?>(
                    It.Is<DeleteProjectCommand>(x => x.Id == 1),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(project);

        var result = await _controller.Delete(1);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public void DeleteProject_WhenProjectIsNotArchived_ReturnsBadRequest()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Heather",
            Slug = "heather",
            ClientName = "Metatron",
            IsArchived = false,
            CompanyId = 1,
        };

        _ = _mediator
            .Setup(m =>
                m.Send<DeleteProjectCommand, Project?>(
                    It.Is<DeleteProjectCommand>(x => x.Id == 1),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new ProjectNotArchivedException(project));

        _ = Assert.ThrowsAsync<ProjectNotArchivedException>(() => _controller.Delete(1));
    }

    [Test]
    public void DeleteProject_WhenProjectDoesNotExist_ReturnsBadRequest()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<DeleteProjectCommand, Project?>(
                    It.Is<DeleteProjectCommand>(x => x.Id == 1),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new ProjectNotFoundException(1));

        _ = Assert.ThrowsAsync<ProjectNotFoundException>(() => _controller.Delete(1));
    }

    [Test]
    public void DeleteProject_InternalServerError()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<DeleteProjectCommand, Project?>(
                    It.IsAny<DeleteProjectCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new Exception("Database error"));

        _ = Assert.ThrowsAsync<Exception>(() => _controller.Delete(1));
    }

    [Test]
    public async Task DeleteProjectBySlug_ReturnsOk()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Heather",
            Slug = "heather",
            ClientName = "Metatron",
            IsArchived = true,
            CompanyId = 1,
        };

        _ = _mediator
            .Setup(m =>
                m.Send<GetProjectIdBySlugQuery, int>(
                    It.Is<GetProjectIdBySlugQuery>(q => q.Slug == "heather"),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        _ = _mediator
            .Setup(m =>
                m.Send<DeleteProjectCommand, Project?>(
                    It.Is<DeleteProjectCommand>(x => x.Id == 1),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(project);

        var result = await _controller.Delete("heather");

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public void DeleteProjectBySlug_WhenProjectDoesNotExist()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<GetProjectIdBySlugQuery, int>(
                    It.Is<GetProjectIdBySlugQuery>(q => q.Slug == "test"),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new ProjectNotFoundException("test"));

        _ = Assert.ThrowsAsync<ProjectNotFoundException>(() => _controller.Delete("test"));
    }

    [Test]
    public void DeleteProjectBySlug_InternalServerError()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<GetProjectIdBySlugQuery, int>(
                    It.IsAny<GetProjectIdBySlugQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.Delete("test"));
    }
}

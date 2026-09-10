using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Projects;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Tests.Projects;

[TestFixture]
public class PutProjectControllerTest
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
    public async Task CreateProject_Test()
    {
        //prepare
        _ = _mediator
            .Setup(m =>
                m.Send<CreateProjectCommand, int>(
                    It.IsAny<CreateProjectCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        var request = new PutProjectRequest(
            ProjectName: "Example Project",
            ClientName: "Example Client",
            CompanyId: 1,
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            IsEoC: false,
            Notes: "Example Notes"
        );
        var result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<PutProjectResponse>());

        var projectResponse = createdResult.Value as PutProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));

            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });
        _mediator.Verify(mediator =>
            mediator.Send<CreateProjectCommand, int>(
                It.Is<CreateProjectCommand>(command =>
                    command.ProjectName == "Example Project"
                    && command.CompanyId == 1
                    && command.ClientName == "Example Client"
                    && command.CompanyState == CompanyState.EXTERNAL
                    && command.IsmsLevel == SecurityLevel.NORMAL
                    && !command.IsEoC
                    && command.Notes == "Example Notes"
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task UpdateProjecTest()
    {
        //prepare
        _ = _mediator
            .Setup(m =>
                m.Send<UpdateProjectCommand, int>(
                    It.IsAny<UpdateProjectCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        var request = new PutProjectRequest(
            ProjectName: "Example Project",
            ClientName: "Example Client",
            CompanyId: 1,
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            IsEoC: false,
            Notes: "Example Notes"
        );
        _ = await _controller.Put(request, 1);
        _mediator.Verify(mediator =>
            mediator.Send<UpdateProjectCommand, int>(
                It.Is<UpdateProjectCommand>(command =>
                    command.ProjectName == "Example Project"
                    && command.ClientName == "Example Client"
                    && command.CompanyId == 1
                    && command.CompanyState == CompanyState.EXTERNAL
                    && command.IsmsLevel == SecurityLevel.NORMAL
                    && !command.IsEoC
                    && command.Notes == "Example Notes"
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task CreateProject_BadRequestTest()
    {
        var request = new PutProjectRequest(
            ProjectName: "",
            ClientName: " ",
            CompanyId: 1,
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            IsEoC: false,
            Notes: ""
        );

        var result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public void CreateProject_BadRequestTest_SlugAlreadyExists()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<CreateProjectCommand, int>(
                    It.IsAny<CreateProjectCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new ProjectSlugAlreadyExistsException("example_project"));

        var request = new PutProjectRequest(
            ProjectName: "Tour Eiffel",
            ClientName: "BusinessUnit 9001",
            CompanyId: 1,
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            IsEoC: false,
            Notes: "Example Notes"
        );

        _ = Assert.ThrowsAsync<ProjectSlugAlreadyExistsException>(() => _controller.Put(request));
    }

    [Test]
    public void CreateProject_MediatorThrowsInvalidOperationExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<CreateProjectCommand, int>(
                    It.IsAny<CreateProjectCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidOperationException("An error message"));
        var request = new PutProjectRequest(
            ProjectName: "p",
            ClientName: "b",
            CompanyId: 1,
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            IsEoC: false,
            Notes: "Example Notes"
        );

        _ = Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Put(request));
    }

    [Test]
    public void CreateProject_MediatorThrowsOtherExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<CreateProjectCommand, int>(
                    It.IsAny<CreateProjectCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));

        var request = new PutProjectRequest(
            ProjectName: "p",
            ClientName: "b",
            CompanyId: 1,
            TeamId: null,
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            IsEoC: false,
            Notes: "Example Notes"
        );

        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.Put(request));
    }

    [Test]
    public async Task ChangeProjectDataControllerTest()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<UpdateProjectCommand, int>(
                    It.IsAny<UpdateProjectCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        var request = new PutProjectRequest(
            ProjectName: "Example Project",
            ClientName: "Example Client",
            CompanyId: 1,
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            IsEoC: false,
            Notes: "Example Notes"
        );
        var result = await _controller.Put(request, 1);

        Assert.That(result, Is.Not.Null);

        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<PutProjectResponse>());

        var projectResponse = createdResult.Value as PutProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));

            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });
        _mediator.Verify(mediator =>
            mediator.Send<UpdateProjectCommand, int>(
                It.Is<UpdateProjectCommand>(command =>
                    command.ProjectName == "Example Project"
                    && command.ClientName == "Example Client"
                    && command.CompanyId == 1
                    && command.CompanyState == CompanyState.EXTERNAL
                    && command.IsmsLevel == SecurityLevel.NORMAL
                    && command.Notes == "Example Notes"
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task UpdateProject_IsArchivedFlag_Test()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<UpdateProjectCommand, int>(
                    It.IsAny<UpdateProjectCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        var request = new PutProjectRequest(
            ProjectName: "Example Project",
            ClientName: "Example Client",
            CompanyId: 2,
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            IsEoC: false,
            Notes: "Example Notes",
            IsArchived: true
        );

        var result = await _controller.Put(request, 1);

        Assert.That(result, Is.Not.Null);
        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<PutProjectResponse>());

        var projectResponse = createdResult.Value as PutProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));
            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });

        _mediator.Verify(mediator =>
            mediator.Send<UpdateProjectCommand, int>(
                It.Is<UpdateProjectCommand>(command =>
                    command.ProjectName == "Example Project"
                    && command.ClientName == "Example Client"
                    && command.CompanyId == 2
                    && command.CompanyState == CompanyState.EXTERNAL
                    && command.IsmsLevel == SecurityLevel.NORMAL
                    && !command.IsEoC
                    && command.Notes == "Example Notes"
                    && command.IsArchived
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task UpdateProjectWithSlug_Test()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<GetProjectIdBySlugQuery, int>(
                    It.IsAny<GetProjectIdBySlugQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(4);

        _ = _mediator
            .Setup(m =>
                m.Send<UpdateProjectCommand, int>(
                    It.IsAny<UpdateProjectCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        var updateRequest = new PutProjectRequest(
            ProjectName: "UpdatedProject",
            ClientName: "Updated Client",
            CompanyId: 1,
            TeamId: 2,
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.HIGH,
            IsEoC: false,
            Notes: "Updated Notes"
        );
        var updateResult = await _controller.Put(updateRequest, "updatedproject");

        Assert.That(updateResult, Is.Not.Null);
        var createdResult = updateResult.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<PutProjectResponse>());

        var projectResponse = createdResult.Value as PutProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));
            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });

        _mediator.Verify(mediator =>
            mediator.Send<UpdateProjectCommand, int>(
                It.Is<UpdateProjectCommand>(command =>
                    command.ProjectName == "UpdatedProject"
                    && command.ClientName == "Updated Client"
                    && command.CompanyId == 1
                    && command.CompanyState == CompanyState.INTERNAL
                    && !command.IsEoC
                    && command.IsmsLevel == SecurityLevel.HIGH
                    && command.Notes == "Updated Notes"
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public void UpdateProjectWithSlug_NotFound_Test()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<GetProjectIdBySlugQuery, int>(
                    It.IsAny<GetProjectIdBySlugQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new ProjectNotFoundException("updatedproject"));
        var updateRequest = new PutProjectRequest(
            ProjectName: "UpdatedProject",
            ClientName: "Updated Business Unit",
            CompanyId: 5,
            TeamId: 2,
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.HIGH,
            IsEoC: false,
            Notes: "Updated Notes"
        );
        _ = Assert.ThrowsAsync<ProjectNotFoundException>(() =>
            _controller.Put(updateRequest, "updatedproject")
        );
    }

    [Test]
    public async Task UpdateProjectWithSlug_IsArchivedFlag_Test()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<GetProjectIdBySlugQuery, int>(
                    It.IsAny<GetProjectIdBySlugQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(4);

        _ = _mediator
            .Setup(m =>
                m.Send<UpdateProjectCommand, int>(
                    It.IsAny<UpdateProjectCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        var updateRequest = new PutProjectRequest(
            ProjectName: "UpdatedProject",
            ClientName: "Updated Client",
            CompanyId: 2,
            TeamId: 2,
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.HIGH,
            IsEoC: false,
            Notes: "Updated Notes",
            IsArchived: true
        );
        var updateResult = await _controller.Put(updateRequest, "updatedproject");

        Assert.That(updateResult, Is.Not.Null);
        var createdResult = updateResult.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<PutProjectResponse>());

        var projectResponse = createdResult.Value as PutProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));
            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });

        _mediator.Verify(mediator =>
            mediator.Send<UpdateProjectCommand, int>(
                It.Is<UpdateProjectCommand>(command =>
                    command.ProjectName == "UpdatedProject"
                    && command.ClientName == "Updated Client"
                    && command.CompanyId == 2
                    && command.CompanyState == CompanyState.INTERNAL
                    && command.IsmsLevel == SecurityLevel.HIGH
                    && command.Notes == "Updated Notes"
                    && command.IsArchived
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }
}

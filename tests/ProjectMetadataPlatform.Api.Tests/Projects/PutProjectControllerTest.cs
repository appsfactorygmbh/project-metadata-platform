﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Projects;
using ProjectMetadataPlatform.Api.Projects.Models;
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
        _mediator
            .Setup(m => m.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var request = new CreateProjectRequest(
            ProjectName: "Example Project",
            ClientName: "Example Client",
            OfferId: "1",
            Company: "Example Comapny",
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            PluginList: [new UpdateProjectPluginRequest("Url", "PluginName", 3)]
        );
        var result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateProjectResponse>());

        var projectResponse = createdResult.Value as CreateProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));

            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });
        _mediator.Verify(mediator =>
            mediator.Send(
                It.Is<CreateProjectCommand>(command =>
                    command.Plugins.Count == 1
                    && command.ProjectName == "Example Project"
                    && command.OfferId == "Example OfferId"
                    && command.Company == "Example Company"
                    && command.ClientName == "Example Client"
                    && command.CompanyState == CompanyState.EXTERNAL
                    && command.IsmsLevel == SecurityLevel.NORMAL
                    && command.Plugins.Single().PluginId == 3
                    && command.Plugins.Single().Url == "Url"
                    && command.Plugins.Single().DisplayName == "PluginName"
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task CreateProjectWithNullProjectPluginList()
    {
        //prepare
        _mediator
            .Setup(m => m.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var request = new CreateProjectRequest(
            ProjectName: "Example Project",
            ClientName: "Example Business Unit",
            OfferId: "1",
            Company: "Example Department",
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL
        );

        await _controller.Put(request);
        _mediator.Verify(mediator =>
            mediator.Send(
                It.Is<CreateProjectCommand>(command =>
                    command.Plugins.Count == 0
                    && command.ProjectName == "Example Project"
                    && command.ClientName == "Example Client"
                    && command.OfferId == "Example OfferId"
                    && command.Company == "Example Company"
                    && command.CompanyState == CompanyState.EXTERNAL
                    && command.IsmsLevel == SecurityLevel.NORMAL
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task UpdateProjectWithNullProjectPluginList()
    {
        //prepare
        _mediator
            .Setup(m => m.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var request = new CreateProjectRequest(
            ProjectName: "Example Project",
            ClientName: "Example Business Unit",
            OfferId: "1",
            Company: "Example Department",
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL
        );
        await _controller.Put(request, 1);
        _mediator.Verify(mediator =>
            mediator.Send(
                It.Is<UpdateProjectCommand>(command =>
                    command.Plugins.Count == 0
                    && command.ProjectName == "Example Project"
                    && command.ClientName == "Example Client"
                    && command.OfferId == "Example OfferId"
                    && command.Company == "Example Company"
                    && command.CompanyState == CompanyState.EXTERNAL
                    && command.IsmsLevel == SecurityLevel.NORMAL
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task CreateProject_BadRequestTest()
    {
        var request = new CreateProjectRequest(
            ProjectName: "",
            ClientName: " ",
            OfferId: "1",
            Company: "",
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL
        );

        var result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public void CreateProject_BadRequestTest_SlugAlreadyExists()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new ProjectSlugAlreadyExistsException("example_project"));

        var request = new CreateProjectRequest(
            ProjectName: "Tour Eiffel",
            ClientName: "BusinessUnit 9001",
            OfferId: "42",
            Company: "Côte-d'Or",
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL
        );

        Assert.ThrowsAsync<ProjectSlugAlreadyExistsException>(() => _controller.Put(request));
    }

    [Test]
    public void CreateProject_MediatorThrowsInvalidOperationExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidOperationException("An error message"));
        var request = new CreateProjectRequest(
            ProjectName: "p",
            ClientName: "b",
            OfferId: "1",
            Company: "d",
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL
        );

        Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Put(request));
    }

    [Test]
    public void CreateProject_MediatorThrowsOtherExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));

        var request = new CreateProjectRequest(
            ProjectName: "p",
            ClientName: "b",
            OfferId: "1",
            Company: "d",
            TeamId: null,
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.NORMAL
        );

        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Put(request));
    }

    [Test]
    public async Task ChangeProjectDataControllerTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var request = new CreateProjectRequest(
            ProjectName: "Example Project",
            ClientName: "Example Business Unit",
            OfferId: "1",
            Company: "Example Department",
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            PluginList: [new UpdateProjectPluginRequest("Url", "PluginName", 3)]
        );
        var result = await _controller.Put(request, 1);

        Assert.That(result, Is.Not.Null);

        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateProjectResponse>());

        var projectResponse = createdResult.Value as CreateProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));

            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });
        _mediator.Verify(mediator =>
            mediator.Send(
                It.Is<UpdateProjectCommand>(command =>
                    command.Plugins.Count == 1
                    && command.ProjectName == "Example Project"
                    && command.ClientName == "Example Client"
                    && command.OfferId == "Example OfferId"
                    && command.Company == "Example Company"
                    && command.CompanyState == CompanyState.EXTERNAL
                    && command.IsmsLevel == SecurityLevel.NORMAL
                    && command.Plugins.Single().PluginId == 3
                    && command.Plugins.Single().Url == "Url"
                    && command.Plugins.Single().DisplayName == "PluginName"
                    && command.Plugins.Single().ProjectId == 1
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task UpdateProject_IsArchivedFlag_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var request = new CreateProjectRequest(
            ProjectName: "Example Project",
            ClientName: "Example Business Unit",
            OfferId: "1",
            Company: "Example Department",
            TeamId: null,
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            PluginList: [new UpdateProjectPluginRequest("Url", "PluginName", 3)],
            IsArchived: true
        );

        var result = await _controller.Put(request, 1);

        Assert.That(result, Is.Not.Null);
        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateProjectResponse>());

        var projectResponse = createdResult.Value as CreateProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));
            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });

        _mediator.Verify(mediator =>
            mediator.Send(
                It.Is<UpdateProjectCommand>(command =>
                    command.Plugins.Count == 1
                    && command.ProjectName == "Example Project"
                    && command.ClientName == "Example Client"
                    && command.OfferId == "Example OfferId"
                    && command.Company == "Example Company"
                    && command.CompanyState == CompanyState.EXTERNAL
                    && command.IsmsLevel == SecurityLevel.NORMAL
                    && command.Plugins.Single().PluginId == 3
                    && command.Plugins.Single().Url == "Url"
                    && command.Plugins.Single().DisplayName == "PluginName"
                    && command.Plugins.Single().ProjectId == 1
                    && command.IsArchived == true
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task UpdateProjectWithSlug_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetProjectIdBySlugQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(4);

        _mediator
            .Setup(m => m.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var updateRequest = new CreateProjectRequest(
            ProjectName: "UpdatedProject",
            ClientName: "Updated Business Unit",
            OfferId: "2",
            Company: "Updated Department",
            TeamId: 2,
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.HIGH,
            PluginList:
            [
                new UpdateProjectPluginRequest("UpdatedUrl", "UpdatedPluginName", 4),
            ]
        );
        var updateResult = await _controller.Put(updateRequest, "updatedproject");

        Assert.That(updateResult, Is.Not.Null);
        var createdResult = updateResult.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateProjectResponse>());

        var projectResponse = createdResult.Value as CreateProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));
            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });

        _mediator.Verify(mediator =>
            mediator.Send(
                It.Is<UpdateProjectCommand>(command =>
                    command.Plugins.Count == 1
                    && command.ProjectName == "UpdatedProject"
                    && command.ClientName == "Updated Client"
                    && command.OfferId == "Updated OfferId"
                    && command.Company == "Updated Company"
                    && command.CompanyState == CompanyState.INTERNAL
                    && command.IsmsLevel == SecurityLevel.HIGH
                    && command.Plugins.Single().PluginId == 4
                    && command.Plugins.Single().Url == "UpdatedUrl"
                    && command.Plugins.Single().DisplayName == "UpdatedPluginName"
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public void UpdateProjectWithSlug_NotFound_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetProjectIdBySlugQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ProjectNotFoundException("updatedproject"));
        var updateRequest = new CreateProjectRequest(
            ProjectName: "UpdatedProject",
            ClientName: "Updated Business Unit",
            OfferId: "2",
            Company: "Updated Department",
            TeamId: 2,
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.HIGH,
            PluginList: new List<UpdateProjectPluginRequest>
            {
                new UpdateProjectPluginRequest("UpdatedUrl", "UpdatedPluginName", 4),
            }
        );
        Assert.ThrowsAsync<ProjectNotFoundException>(() =>
            _controller.Put(updateRequest, "updatedproject")
        );
    }

    [Test]
    public async Task UpdateProjectWithSlug_IsArchivedFlag_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetProjectIdBySlugQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(4);

        _mediator
            .Setup(m => m.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var updateRequest = new CreateProjectRequest(
            ProjectName: "UpdatedProject",
            ClientName: "Updated Business Unit",
            OfferId: "2",
            Company: "Updated Department",
            TeamId: 2,
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.HIGH,
            PluginList:
            [
                new UpdateProjectPluginRequest("UpdatedUrl", "UpdatedPluginName", 4),
            ],
            IsArchived: true
        );
        var updateResult = await _controller.Put(updateRequest, "updatedproject");

        Assert.That(updateResult, Is.Not.Null);
        var createdResult = updateResult.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreateProjectResponse>());

        var projectResponse = createdResult.Value as CreateProjectResponse;
        Assert.That(projectResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResponse.Id, Is.EqualTo(1));
            Assert.That(createdResult.Location, Is.EqualTo("/Projects/1"));
        });

        _mediator.Verify(mediator =>
            mediator.Send(
                It.Is<UpdateProjectCommand>(command =>
                    command.Plugins.Count == 1
                    && command.ProjectName == "UpdatedProject"
                    && command.ClientName == "Updated Client"
                    && command.OfferId == "Updated OfferId"
                    && command.Company == "Updated Company"
                    && command.CompanyState == CompanyState.INTERNAL
                    && command.IsmsLevel == SecurityLevel.HIGH
                    && command.Plugins.Single().PluginId == 4
                    && command.Plugins.Single().Url == "UpdatedUrl"
                    && command.Plugins.Single().DisplayName == "UpdatedPluginName"
                    && command.IsArchived == true
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }
}

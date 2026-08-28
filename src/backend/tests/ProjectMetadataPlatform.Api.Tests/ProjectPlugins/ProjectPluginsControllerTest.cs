using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Common.Models;
using ProjectMetadataPlatform.Api.ProjectPlugins;
using ProjectMetadataPlatform.Api.ProjectPlugins.Models;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.ProjectPlugins;
using ProjectMetadataPlatform.Application.ProjectPlugins.Models;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Api.Tests.ProjectPlugins;

[TestFixture]
public class ProjectPluginsControllerTest
{
    private ProjectPluginsController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<GetProjectIdBySlugQuery, int>(
                    It.IsAny<GetProjectIdBySlugQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        _controller = new ProjectPluginsController(_mediator.Object);
    }

    [Test]
    public async Task GetProjectPlugins_EmptyResponseTest()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(([], []));
        var result = await _controller.GetPlugins(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetProjectPluginResponse>>());

        var getProjectPluginsResponseList = (
            okResult.Value as GetListResponse<GetProjectPluginResponse>
        )!.Resources.ToList();
        Assert.That(getProjectPluginsResponseList, Is.Not.Null);

        Assert.That(getProjectPluginsResponseList, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetProjectPlugins_ListResponse()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                (
                    [
                        new ProjectPluginPermissionModel(
                            new()
                            {
                                Id = 1,
                                PluginId = 1,
                                DisplayName = "ProjectPlugin1",
                                Url = "Url1",
                                Plugin = new() { PluginName = "PluginName1" },
                            },
                            [],
                            []
                        ),
                        new ProjectPluginPermissionModel(
                            new()
                            {
                                Id = 2,
                                PluginId = 1,
                                DisplayName = "ProjectPlugin2",
                                Url = "Url2",
                                Plugin = new() { PluginName = "PluginName2" },
                            },
                            [],
                            []
                        ),
                    ],
                    []
                )
            );
        var result = await _controller.GetPlugins(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetProjectPluginResponse>>());

        var getProjectPluginsResponseList = (
            okResult.Value as GetListResponse<GetProjectPluginResponse>
        )!.Resources.ToList();
        Assert.That(getProjectPluginsResponseList, Is.Not.Null);

        Assert.That(getProjectPluginsResponseList, Has.Count.EqualTo(2));
        Assert.That(getProjectPluginsResponseList[0].Id, Is.EqualTo(1));
        Assert.That(getProjectPluginsResponseList[0].DisplayName, Is.EqualTo("ProjectPlugin1"));
        Assert.That(getProjectPluginsResponseList[0].Url, Is.EqualTo("Url1"));
        Assert.That(getProjectPluginsResponseList[1].Id, Is.EqualTo(2));
        Assert.That(getProjectPluginsResponseList[1].DisplayName, Is.EqualTo("ProjectPlugin2"));
        Assert.That(getProjectPluginsResponseList[1].Url, Is.EqualTo("Url2"));
    }

    [Test]
    public async Task GetProjectPlugins_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<
                    GetAllPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.GetPlugins(1));
    }

    [Test]
    public async Task GetUnarchivedProjectPlugins_EmptyResponseTest()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllUnarchivedPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(
                    It.IsAny<GetAllUnarchivedPluginsForProjectIdQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(([], []));
        var result = await _controller.GetUnarchivedPlugins(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetProjectPluginResponse>>());

        var getProjectPluginsResponseList = (
            okResult.Value as GetListResponse<GetProjectPluginResponse>
        )!.Resources.ToList();
        Assert.That(getProjectPluginsResponseList, Is.Not.Null);

        Assert.That(getProjectPluginsResponseList, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetUnarchivedProjectPlugins_ListResponse()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllUnarchivedPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(
                    It.IsAny<GetAllUnarchivedPluginsForProjectIdQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    [
                        new ProjectPluginPermissionModel(
                            new()
                            {
                                Id = 1,
                                PluginId = 1,
                                DisplayName = "ProjectPlugin1",
                                Url = "Url1",
                                Plugin = new() { PluginName = "PluginName1" },
                            },
                            [],
                            []
                        ),
                        new ProjectPluginPermissionModel(
                            new()
                            {
                                Id = 2,
                                PluginId = 1,
                                DisplayName = "ProjectPlugin2",
                                Url = "Url2",
                                Plugin = new() { PluginName = "PluginName2" },
                            },
                            [],
                            []
                        ),
                    ],
                    []
                )
            );
        var result = await _controller.GetUnarchivedPlugins(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetProjectPluginResponse>>());

        var getProjectPluginsResponseList = (
            okResult.Value as GetListResponse<GetProjectPluginResponse>
        )!.Resources.ToList();
        Assert.That(getProjectPluginsResponseList, Is.Not.Null);

        Assert.That(getProjectPluginsResponseList, Has.Count.EqualTo(2));
        Assert.That(getProjectPluginsResponseList[0].Id, Is.EqualTo(1));
        Assert.That(getProjectPluginsResponseList[0].DisplayName, Is.EqualTo("ProjectPlugin1"));
        Assert.That(getProjectPluginsResponseList[0].Url, Is.EqualTo("Url1"));
        Assert.That(getProjectPluginsResponseList[1].Id, Is.EqualTo(2));
        Assert.That(getProjectPluginsResponseList[1].DisplayName, Is.EqualTo("ProjectPlugin2"));
        Assert.That(getProjectPluginsResponseList[1].Url, Is.EqualTo("Url2"));
    }

    [Test]
    public async Task GetUnarchivedProjectPlugins_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<
                    GetAllUnarchivedPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(
                    It.IsAny<GetAllUnarchivedPluginsForProjectIdQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.GetUnarchivedPlugins(1));
    }

    [Test]
    public async Task GetProjectPluginsBySlug_EmptyResponseTest()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(([], []));
        var result = await _controller.GetPluginsBySlug("Slug");
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetProjectPluginResponse>>());

        var getProjectPluginsResponseList = (
            okResult.Value as GetListResponse<GetProjectPluginResponse>
        )!.Resources.ToList();
        Assert.That(getProjectPluginsResponseList, Is.Not.Null);

        Assert.That(getProjectPluginsResponseList, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetProjectPluginsBySlug_ListResponse()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                (
                    [
                        new ProjectPluginPermissionModel(
                            new()
                            {
                                Id = 1,
                                PluginId = 1,
                                DisplayName = "ProjectPlugin1",
                                Url = "Url1",
                                Plugin = new() { PluginName = "PluginName1" },
                            },
                            [],
                            []
                        ),
                        new ProjectPluginPermissionModel(
                            new()
                            {
                                Id = 2,
                                PluginId = 1,
                                DisplayName = "ProjectPlugin2",
                                Url = "Url2",
                                Plugin = new() { PluginName = "PluginName2" },
                            },
                            [],
                            []
                        ),
                    ],
                    []
                )
            );
        var result = await _controller.GetPluginsBySlug("Slug");
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetProjectPluginResponse>>());

        var getProjectPluginsResponseList = (
            okResult.Value as GetListResponse<GetProjectPluginResponse>
        )!.Resources.ToList();
        Assert.That(getProjectPluginsResponseList, Is.Not.Null);

        Assert.That(getProjectPluginsResponseList, Has.Count.EqualTo(2));
        Assert.That(getProjectPluginsResponseList[0].Id, Is.EqualTo(1));
        Assert.That(getProjectPluginsResponseList[0].DisplayName, Is.EqualTo("ProjectPlugin1"));
        Assert.That(getProjectPluginsResponseList[0].Url, Is.EqualTo("Url1"));
        Assert.That(getProjectPluginsResponseList[1].Id, Is.EqualTo(2));
        Assert.That(getProjectPluginsResponseList[1].DisplayName, Is.EqualTo("ProjectPlugin2"));
        Assert.That(getProjectPluginsResponseList[1].Url, Is.EqualTo("Url2"));
    }

    [Test]
    public async Task GetProjectPluginsBySlug_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<
                    GetAllPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(It.IsAny<GetAllPluginsForProjectIdQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.GetPluginsBySlug("Slug"));
    }

    [Test]
    public async Task GetUnarchivedProjectPluginsBySlug_EmptyResponseTest()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllUnarchivedPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(
                    It.IsAny<GetAllUnarchivedPluginsForProjectIdQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(([], []));
        var result = await _controller.GetUnarchivedPluginsBySlug("Slug");
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetProjectPluginResponse>>());

        var getProjectPluginsResponseList = (
            okResult.Value as GetListResponse<GetProjectPluginResponse>
        )!.Resources.ToList();
        Assert.That(getProjectPluginsResponseList, Is.Not.Null);

        Assert.That(getProjectPluginsResponseList, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetUnarchivedProjectPluginsBySlug_ListResponse()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllUnarchivedPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(
                    It.IsAny<GetAllUnarchivedPluginsForProjectIdQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    [
                        new ProjectPluginPermissionModel(
                            new()
                            {
                                Id = 1,
                                PluginId = 1,
                                DisplayName = "ProjectPlugin1",
                                Url = "Url1",
                                Plugin = new() { PluginName = "PluginName1" },
                            },
                            [],
                            []
                        ),
                        new ProjectPluginPermissionModel(
                            new()
                            {
                                Id = 2,
                                PluginId = 1,
                                DisplayName = "ProjectPlugin2",
                                Url = "Url2",
                                Plugin = new() { PluginName = "PluginName2" },
                            },
                            [],
                            []
                        ),
                    ],
                    []
                )
            );
        var result = await _controller.GetUnarchivedPluginsBySlug("Slug");
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetProjectPluginResponse>>());

        var getProjectPluginsResponseList = (
            okResult.Value as GetListResponse<GetProjectPluginResponse>
        )!.Resources.ToList();
        Assert.That(getProjectPluginsResponseList, Is.Not.Null);

        Assert.That(getProjectPluginsResponseList, Has.Count.EqualTo(2));
        Assert.That(getProjectPluginsResponseList[0].Id, Is.EqualTo(1));
        Assert.That(getProjectPluginsResponseList[0].DisplayName, Is.EqualTo("ProjectPlugin1"));
        Assert.That(getProjectPluginsResponseList[0].Url, Is.EqualTo("Url1"));
        Assert.That(getProjectPluginsResponseList[1].Id, Is.EqualTo(2));
        Assert.That(getProjectPluginsResponseList[1].DisplayName, Is.EqualTo("ProjectPlugin2"));
        Assert.That(getProjectPluginsResponseList[1].Url, Is.EqualTo("Url2"));
    }

    [Test]
    public async Task GetUnarchivedProjectPluginsBySlug_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<
                    GetAllUnarchivedPluginsForProjectIdQuery,
                    (
                        IEnumerable<ProjectPluginPermissionModel>,
                        IEnumerable<AuthorizationConstants.Actions>
                    )
                >(
                    It.IsAny<GetAllUnarchivedPluginsForProjectIdQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.GetUnarchivedPluginsBySlug("Slug")
        );
    }

    [Test]
    public async Task AddProjectPlugin_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<AddProjectPluginCommand, int>(
                    It.IsAny<AddProjectPluginCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.AddProjectPlugin(new AddProjectPluginRequest("a", "Displayname", 1), 1)
        );
    }

    [Test]
    public async Task AddProjectPlugin_WhiteSpaceName_BadRequestTest()
    {
        var result = await _controller.AddProjectPlugin(
            new AddProjectPluginRequest("", "Displayname", 1),
            1
        );
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task AddProjectPlugin_ReturnsIdTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<AddProjectPluginCommand, int>(
                    It.IsAny<AddProjectPluginCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        var result = await _controller.AddProjectPlugin(
            new AddProjectPluginRequest("a", "Displayname", 1),
            1
        );
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Location, Is.EqualTo("/Projects/1/plugins/1"));
        Assert.That(createdResult.Value, Is.InstanceOf<AddProjectPluginResponse>());

        var createProjectPluginResponse = createdResult.Value as AddProjectPluginResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createProjectPluginResponse, Is.Not.Null);
            Assert.That(createProjectPluginResponse!.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task AddProjectPluginBySlug_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<AddProjectPluginCommand, int>(
                    It.IsAny<AddProjectPluginCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.AddProjectPluginBySlug(
                new AddProjectPluginRequest("a", "Displayname", 1),
                "Slug"
            )
        );
    }

    [Test]
    public async Task AddProjectPluginBySlug_WhiteSpaceName_BadRequestTest()
    {
        var result = await _controller.AddProjectPluginBySlug(
            new AddProjectPluginRequest("", "Displayname", 1),
            "Slug"
        );
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task AddProjectPluginBySlug_ReturnsIdTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<AddProjectPluginCommand, int>(
                    It.IsAny<AddProjectPluginCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        var result = await _controller.AddProjectPluginBySlug(
            new AddProjectPluginRequest("a", "Displayname", 1),
            "Slug"
        );
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Location, Is.EqualTo("/Projects/Slug/plugins/1"));
        Assert.That(createdResult.Value, Is.InstanceOf<AddProjectPluginResponse>());

        var createProjectPluginResponse = createdResult.Value as AddProjectPluginResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createProjectPluginResponse, Is.Not.Null);
            Assert.That(createProjectPluginResponse!.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task UpdateProjectPlugin_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<UpdateProjectPluginCommand, ProjectPlugin>(
                    It.IsAny<UpdateProjectPluginCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.UpdateProjectPlugin(new UpdateProjectPluginRequest("A", "A"), 1, 1)
        );
    }

    [Test]
    public async Task UpdateProjectPluginBySlug_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<UpdateProjectPluginCommand, ProjectPlugin>(
                    It.IsAny<UpdateProjectPluginCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.UpdateProjectPluginBySlug(
                new UpdateProjectPluginRequest("B", "B"),
                "Slug",
                1
            )
        );
    }

    [Test]
    public async Task UpdateProjectPlugin_WhiteSpaceUrlBadRequestTest()
    {
        var result = await _controller.UpdateProjectPlugin(
            new UpdateProjectPluginRequest("", ""),
            1,
            1
        );
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateProjectPlugin_ReturnsUpdatedProjectPluginTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<UpdateProjectPluginCommand, ProjectPlugin>(
                    It.IsAny<UpdateProjectPluginCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ProjectPlugin
                {
                    PluginId = 1,
                    DisplayName = "ProjectPlugin",
                    Url = "A Url",
                    Id = 1,
                    Plugin = new() { PluginName = "PluginName1" },
                }
            );
        var result = await _controller.UpdateProjectPlugin(
            new UpdateProjectPluginRequest("Url", "Displayname"),
            1,
            1
        );
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetProjectPluginResponse>());

        var updateProjectPluginResponse = okResult.Value as GetProjectPluginResponse;

        Assert.Multiple(() =>
        {
            Assert.That(updateProjectPluginResponse, Is.Not.Null);
            Assert.That(updateProjectPluginResponse!.DisplayName, Is.EqualTo("ProjectPlugin"));
            Assert.That(updateProjectPluginResponse.Url, Is.EqualTo("A Url"));
            Assert.That(updateProjectPluginResponse.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task UpdateProjectPluginBySlug_WhiteSpaceUrl_BadRequestTest()
    {
        var result = await _controller.UpdateProjectPluginBySlug(
            new UpdateProjectPluginRequest("", ""),
            "Slug",
            1
        );
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateProjectPluginBySlug_ReturnsUpdatedProjectPluginTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<UpdateProjectPluginCommand, ProjectPlugin>(
                    It.IsAny<UpdateProjectPluginCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ProjectPlugin
                {
                    PluginId = 1,
                    DisplayName = "ProjectPlugin",
                    Url = "A Url",
                    Id = 1,
                    Plugin = new() { PluginName = "PluginName1" },
                }
            );

        var result = await _controller.UpdateProjectPluginBySlug(
            new UpdateProjectPluginRequest("Url", "Displayname"),
            "Slug",
            1
        );
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetProjectPluginResponse>());

        var updateProjectPluginResponse = okResult.Value as GetProjectPluginResponse;

        Assert.Multiple(() =>
        {
            Assert.That(updateProjectPluginResponse, Is.Not.Null);
            Assert.That(updateProjectPluginResponse!.DisplayName, Is.EqualTo("ProjectPlugin"));
            Assert.That(updateProjectPluginResponse.Url, Is.EqualTo("A Url"));
            Assert.That(updateProjectPluginResponse.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task DeleteProjectPlugin_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<DeleteProjectPluginCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.DeleteProjectPlugin(1, 1));
    }

    [Test]
    public async Task DeleteProjectPlugin_NoContentResponseTest()
    {
        var result = await _controller.DeleteProjectPlugin(1, 1);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task DeleteProjectPluginBySlug_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<DeleteProjectPluginCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.DeleteProjectPluginBySlug("Project", 1)
        );
    }

    [Test]
    public async Task DeleteProjectPluginBySlug_NoContentResponseTest()
    {
        var result = await _controller.DeleteProjectPluginBySlug("Project", 1);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.ProjectPlugins;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Tests.ProjectPlugins;

public class GetAllUnarchivedPluginsForProjectIdQueryHandlerTest
{
    private GetAllUnarchivedPluginsForProjectIdQueryHandler _handler;
    private Mock<IPluginRepository> _pluginRepositoryMock;
    private Mock<IBillingRepository> _mockBillingRepo;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void SetUp()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockBillingRepo = new Mock<IBillingRepository>();
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _handler = new GetAllUnarchivedPluginsForProjectIdQueryHandler(
            _pluginRepositoryMock.Object,
            billingRepository: _mockBillingRepo.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task Handle_WhenUnarchivedPluginsExist_ReturnsPlugins()
    {
        var plugins = new List<ProjectPlugin>
        {
            new()
            {
                PluginId = 1,
                Plugin = new Plugin
                {
                    Id = 1,
                    PluginName = "Plugin 1",
                    IsArchived = false,
                }, // Unarchived
                ProjectId = 1,
                Url = "Plugin1.com",
                DisplayName = "GitLab",
            },
            new()
            {
                PluginId = 2,
                Plugin = new Plugin
                {
                    Id = 2,
                    PluginName = "Plugin 2",
                    IsArchived = false,
                }, // Unarchived
                ProjectId = 1,
                Url = "Plugin2.com",
                DisplayName = "Jira",
            },
        };

        _ = _pluginRepositoryMock
            .Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync(plugins.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<ProjectPlugin>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<ProjectPlugin> query, Dictionary<string, string>? dict) => query
            );
        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);
        var result = (await _handler.Handle(query, It.IsAny<CancellationToken>())).Item1.ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2)); // Expecting two unarchived plugins
        Assert.Multiple(() =>
        {
            Assert.That(result[0].Plugin.Plugin?.PluginName, Is.EqualTo("Plugin 1"));
            Assert.That(result[0].Plugin.Url, Is.EqualTo("Plugin1.com"));
            Assert.That(result[1].Plugin.Plugin?.PluginName, Is.EqualTo("Plugin 2"));
            Assert.That(result[1].Plugin.Url, Is.EqualTo("Plugin2.com"));
        });
    }

    [Test]
    public async Task Handle_WhenNoUnarchivedPluginsExist_ReturnsEmptyList()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<ProjectPlugin>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<ProjectPlugin> query, Dictionary<string, string>? dict) => query
            );
        var plugins = new List<ProjectPlugin>(); // No plugins found
        _ = _pluginRepositoryMock
            .Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync(plugins.BuildMock());

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);
        var result = (await _handler.Handle(query, It.IsAny<CancellationToken>())).Item1.ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(0)); // Expecting an empty list
    }

    [Test]
    public async Task Handle_WhenSomePluginsAreArchived_ReturnsOnlyUnarchivedPlugins()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<ProjectPlugin>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<ProjectPlugin> query, Dictionary<string, string>? dict) => query
            );
        var plugins = new List<ProjectPlugin>
        {
            new()
            {
                PluginId = 1,
                Plugin = new Plugin
                {
                    Id = 1,
                    PluginName = "Plugin 1",
                    IsArchived = false,
                }, // Unarchived
                ProjectId = 1,
                Url = "Plugin1.com",
                DisplayName = "GitLab",
            },
            new()
            {
                PluginId = 2,
                Plugin = new Plugin
                {
                    Id = 2,
                    PluginName = "Plugin 2",
                    IsArchived = true,
                }, // Archived
                ProjectId = 1,
                Url = "Plugin2.com",
                DisplayName = "Jira",
            },
        };

        _ = _pluginRepositoryMock
            .Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync(plugins.Where(p => !p.Plugin!.IsArchived).ToList().BuildMock());

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);
        var result = (await _handler.Handle(query, It.IsAny<CancellationToken>())).Item1.ToList();

        Assert.That(result, Has.Count.EqualTo(1)); // Only one unarchived plugin should be returned
        Assert.That(result[0].Plugin.Plugin?.PluginName, Is.EqualTo("Plugin 1")); // Assert the unarchived plugin is "Plugin 1"
    }

    [Test]
    public void Handle_WhenProjectDoesNotExist_ThrowsArgumentException()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ProjectPlugin>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _pluginRepositoryMock
            .Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new ArgumentException("Project with Id 999 does not exist."));

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(999); // Non-existent project ID

        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            _ = await _handler.Handle(query, It.IsAny<CancellationToken>());
        });

        Assert.That(ex.Message, Is.EqualTo("Project with Id 999 does not exist."));
    }
}

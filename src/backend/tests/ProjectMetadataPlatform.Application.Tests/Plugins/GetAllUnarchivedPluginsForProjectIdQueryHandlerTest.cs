using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

public class GetAllUnarchivedPluginsForProjectIdQueryHandlerTest
{
    private GetAllUnarchivedPluginsForProjectIdQueryHandler _handler;
    private Mock<IPluginRepository> _pluginRepositoryMock;

    [SetUp]
    public void SetUp()
    {
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _handler = new GetAllUnarchivedPluginsForProjectIdQueryHandler(
            _pluginRepositoryMock.Object
        );
    }

    [Test]
    public async Task Handle_WhenUnarchivedPluginsExist_ReturnsPlugins()
    {
        var plugins = new List<ProjectPlugins>
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

        _pluginRepositoryMock
            .Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync(plugins);

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2)); // Expecting two unarchived plugins
        Assert.Multiple(() =>
        {
            Assert.That(result[0].Plugin?.PluginName, Is.EqualTo("Plugin 1"));
            Assert.That(result[0].Url, Is.EqualTo("Plugin1.com"));
            Assert.That(result[1].Plugin?.PluginName, Is.EqualTo("Plugin 2"));
            Assert.That(result[1].Url, Is.EqualTo("Plugin2.com"));
        });
    }

    [Test]
    public async Task Handle_WhenNoUnarchivedPluginsExist_ReturnsEmptyList()
    {
        var plugins = new List<ProjectPlugins>(); // No plugins found
        _pluginRepositoryMock
            .Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync(plugins);

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(0)); // Expecting an empty list
    }

    [Test]
    public async Task Handle_WhenCancellationTokenIsTriggered_AbortsOperation()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync(); // Trigger cancellation immediately

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);

        _pluginRepositoryMock
            .Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync(new List<ProjectPlugins>());

        var ex = Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await _handler.Handle(query, cancellationTokenSource.Token);
        });

        Assert.That(ex, Is.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public async Task Handle_WhenSomePluginsAreArchived_ReturnsOnlyUnarchivedPlugins()
    {
        var plugins = new List<ProjectPlugins>
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

        _pluginRepositoryMock
            .Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(1))
            .ReturnsAsync(plugins.Where(p => !p.Plugin!.IsArchived).ToList());

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(1);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Has.Count.EqualTo(1)); // Only one unarchived plugin should be returned
        Assert.That(result[0].Plugin?.PluginName, Is.EqualTo("Plugin 1")); // Assert the unarchived plugin is "Plugin 1"
    }

    [Test]
    public void Handle_WhenProjectDoesNotExist_ThrowsArgumentException()
    {
        _pluginRepositoryMock
            .Setup(r => r.GetAllUnarchivedPluginsForProjectIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new ArgumentException("Project with Id 999 does not exist."));

        var query = new GetAllUnarchivedPluginsForProjectIdQuery(999); // Non-existent project ID

        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _handler.Handle(query, It.IsAny<CancellationToken>());
        });

        Assert.That(ex.Message, Is.EqualTo("Project with Id 999 does not exist."));
    }
}

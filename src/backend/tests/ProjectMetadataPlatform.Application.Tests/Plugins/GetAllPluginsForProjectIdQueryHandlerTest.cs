using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

[TestFixture]
public class GetAllPluginsForProjectIdQueryHandlerTest
{
    [SetUp]
    public void SetUp()
    {
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _handler = new GetAllPluginsForProjectIdQueryHandler(_pluginRepositoryMock.Object);
    }

    private GetAllPluginsForProjectIdQueryHandler _handler;
    private Mock<IPluginRepository> _pluginRepositoryMock;

    [Test]
    public async Task HandleGetAllProjectsForProjectIdQueryHandlerTest()
    {
        // Arrange
        var plugins = new List<ProjectPlugins>
        {
            new()
            {
                PluginId = 1,
                Plugin = new Plugin { Id = 1, PluginName = "Plugin 1" },
                ProjectId = 1,
                Project = new Project
                {
                    Id = 1,
                    ProjectName = "Project 1",
                    Slug = "project 1",
                    ClientName = "Client 1",
                },
                Url = "Plugin1.com",
            },
            new()
            {
                PluginId = 2,
                Plugin = new Plugin { Id = 2, PluginName = "Plugin 2" },
                ProjectId = 1,
                Project = new Project
                {
                    Id = 1,
                    ProjectName = "Project 1",
                    Slug = "project 1",
                    ClientName = "Client 1",
                },
                Url = "Plugin2.com",
            },
        };
        _pluginRepositoryMock.Setup(r => r.GetAllPluginsForProjectIdAsync(1)).ReturnsAsync(plugins);

        var query = new GetAllPluginsForProjectIdQuery(1);
        var result = (await _handler.Handle(query, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<List<ProjectPlugins>>());
        Assert.That(result, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(result[0].Url, Is.EqualTo("Plugin1.com"));
            Assert.That(result[0].Plugin?.PluginName, Is.EqualTo("Plugin 1"));
            Assert.That(result[1].Url, Is.EqualTo("Plugin2.com"));
            Assert.That(result[1].Plugin?.PluginName, Is.EqualTo("Plugin 2"));
        });

        //test for no plugins
        var queryFail = new GetAllPluginsForProjectIdQuery(0);
        var resultFail = await _handler.Handle(queryFail, It.IsAny<CancellationToken>());
        Assert.That(resultFail, Is.Null);
    }

    [Test]
    public async Task HandleGetAllProjectsForProjectIdQueryHandler_WhenZeroPlugins_Test()
    {
        var queryFail = new GetAllPluginsForProjectIdQuery(0);
        var resultFail = await _handler.Handle(queryFail, It.IsAny<CancellationToken>());
        Assert.That(resultFail, Is.Null);
    }
}

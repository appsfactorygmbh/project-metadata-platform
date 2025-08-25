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

public class GetGlobalPluginsQueryHandlerTest
{
    private GetGlobalPluginsQueryHandler _handler;
    private Mock<IPluginRepository> _pluginRepositoryMock;

    [SetUp]
    public void SetUp()
    {
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _handler = new GetGlobalPluginsQueryHandler(_pluginRepositoryMock.Object);
    }

    [Test]
    public async Task HandleGetGlobalPluginsQueryHandler_Test()
    {
        // Arrange
        var plugins = new List<Plugin>
        {
            new()
            {
                Id = 1,
                PluginName = "plugin 1",
                IsArchived = false,
            },
            new()
            {
                Id = 2,
                PluginName = "plugin 2",
                IsArchived = false,
            },
        };
        _pluginRepositoryMock.Setup(r => r.GetGlobalPluginsAsync()).ReturnsAsync(plugins);

        var query = new GetGlobalPluginsQuery();
        var result = (await _handler.Handle(query, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<List<Plugin>>());
        Assert.That(result, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[1].Id, Is.EqualTo(2));
        });

        Assert.Multiple(() =>
        {
            Assert.That(result[0].PluginName, Is.EqualTo("plugin 1"));
            Assert.That(result[1].PluginName, Is.EqualTo("plugin 2"));
        });

        Assert.Multiple(() =>
        {
            Assert.That(result[0].IsArchived, Is.False);
            Assert.That(result[1].IsArchived, Is.False);
        });
    }

    [Test]
    public async Task HandleGetGlobalPluginsQueryHandler_WhenZeroPlugins_Test()
    {
        var queryFail = new GetGlobalPluginsQuery();
        var resultFail = await _handler.Handle(queryFail, It.IsAny<CancellationToken>());
        Assert.That(resultFail, Is.Empty);
    }
}

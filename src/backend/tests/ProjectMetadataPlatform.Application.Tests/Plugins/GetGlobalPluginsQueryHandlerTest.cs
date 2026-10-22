using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

public class GetGlobalPluginsQueryHandlerTest
{
    private GetGlobalPluginsQueryHandler _handler;
    private Mock<IPluginRepository> _pluginRepositoryMock;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IPaginationHelper> _paginationHelperMock;

    [SetUp]
    public void SetUp()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelperMock = new Mock<IPaginationHelper>();
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _handler = new GetGlobalPluginsQueryHandler(
            _pluginRepositoryMock.Object,
            authorizationService: _authorizationServiceMock.Object,
            _paginationHelperMock.Object
        );
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
        _ = _pluginRepositoryMock
            .Setup(r => r.GetGlobalPluginsAsync(null))
            .ReturnsAsync(plugins.BuildMock());
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Plugin>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Plugin> query, int? limit) => (query, null));
        var query = new GetGlobalPluginsQuery(null, null);
        var result = (await _handler.Handle(query, It.IsAny<CancellationToken>())).Item1.ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(
            result,
            Is.TypeOf<List<(Plugin, IEnumerable<AuthorizationConstants.Actions>)>>()
        );
        Assert.That(result, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(result[0].plugin.Id, Is.EqualTo(1));
            Assert.That(result[1].plugin.Id, Is.EqualTo(2));
        });

        Assert.Multiple(() =>
        {
            Assert.That(result[0].plugin.PluginName, Is.EqualTo("plugin 1"));
            Assert.That(result[1].plugin.PluginName, Is.EqualTo("plugin 2"));
        });

        Assert.Multiple(() =>
        {
            Assert.That(result[0].plugin.IsArchived, Is.False);
            Assert.That(result[1].plugin.IsArchived, Is.False);
        });
    }

    [Test]
    public async Task HandleGetGlobalPluginsQueryHandler_WhenZeroPlugins_Test()
    {
        _ = _pluginRepositoryMock
            .Setup(r => r.GetGlobalPluginsAsync(null))
            .ReturnsAsync(new List<Plugin> { }.BuildMock());
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Plugin>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Plugin> query, int? limit) => (query, null));
        var queryFail = new GetGlobalPluginsQuery(null, null);
        var resultFail = await _handler.Handle(queryFail, It.IsAny<CancellationToken>());
        Assert.That(resultFail.Item1, Is.Empty);
    }
}

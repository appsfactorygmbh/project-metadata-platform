using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.ProjectPlugins;
using ProjectMetadataPlatform.Application.ProjectPlugins.Models;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.ProjectPlugins;

[TestFixture]
public class GetAllPluginsForProjectIdQueryHandlerTest
{
    [SetUp]
    public void SetUp()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _mockBillingRepo = new Mock<IBillingRepository>();
        _handler = new GetAllPluginsForProjectIdQueryHandler(
            _pluginRepositoryMock.Object,
            billingRepository: _mockBillingRepo.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    private GetAllPluginsForProjectIdQueryHandler _handler;
    private Mock<IPluginRepository> _pluginRepositoryMock;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IBillingRepository> _mockBillingRepo;

    [Test]
    public async Task HandleGetAllProjectsForProjectIdQueryHandlerTest()
    {
        // Arrange
        var plugins = new List<ProjectPlugin>
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
                    CompanyId = 1,
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
                    CompanyId = 1,
                },
                Url = "Plugin2.com",
            },
        };
        _ = _pluginRepositoryMock
            .Setup(r => r.GetAllPluginsForProjectIdAsync(1))
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
        var query = new GetAllPluginsForProjectIdQuery(1);
        var result = (await _handler.Handle(query, It.IsAny<CancellationToken>())).Item1.ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<List<ProjectPluginPermissionModel>>());
        Assert.That(result, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(result[0].Plugin.Url, Is.EqualTo("Plugin1.com"));
            Assert.That(result[0].Plugin.Plugin?.PluginName, Is.EqualTo("Plugin 1"));
            Assert.That(result[1].Plugin.Url, Is.EqualTo("Plugin2.com"));
            Assert.That(result[1].Plugin.Plugin?.PluginName, Is.EqualTo("Plugin 2"));
        });
    }

    [Test]
    public async Task HandleGetAllProjectsForProjectIdQueryHandler_WhenZeroPlugins_Test()
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
        _ = _pluginRepositoryMock
            .Setup(r => r.GetAllPluginsForProjectIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<ProjectPlugin> { }.BuildMock());
        var queryFail = new GetAllPluginsForProjectIdQuery(0);
        var resultFail = (await _handler.Handle(queryFail, It.IsAny<CancellationToken>())).Item1;
        Assert.That(resultFail, Is.Empty);
    }
}

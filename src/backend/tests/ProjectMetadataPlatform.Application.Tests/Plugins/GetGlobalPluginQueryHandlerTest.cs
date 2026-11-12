using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

[TestFixture]
public class GetGlobalPluginQueryHandlerTest
{
    private GetGlobalPluginQueryHandler _handler;
    private Mock<IPluginRepository> _pluginRepositoryMock;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void SetUp()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _pluginRepositoryMock = new Mock<IPluginRepository>();
        _handler = new GetGlobalPluginQueryHandler(
            _pluginRepositoryMock.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetGlobalPlugin_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnPlugin = new Plugin
        {
            Id = 1,
            PluginName = "plugin 1",
            IsArchived = false,
            BaseUrl = "https://plugin1.com",
        };

        _ = _pluginRepositoryMock
            .Setup(repo => repo.GetPluginByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(returnPlugin);
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Plugin>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        // Act
        var result = await _handler.Handle(
            new GetGlobalPluginQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1, Is.Not.Null);
        Assert.That(result.Item1, Is.EqualTo(returnPlugin));
        _pluginRepositoryMock.Verify(
            m => m.GetPluginByIdAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
    }

    [Test]
    public async Task GetGlobalPlugin_ThrowPluginNotFoundException_IfPluginNotFoundAsync()
    {
        // Arrange
        _ = _pluginRepositoryMock
            .Setup(repo => repo.GetPluginByIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new PluginNotFoundException(1));
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Plugin>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        // Act + Assert
        var ex = await Assert.ThrowsAsync<PluginNotFoundException>(async () =>
            await _handler.Handle(new GetGlobalPluginQuery(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task GetGlobalPlugin_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Plugin>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);

        var request = new GetGlobalPluginQuery(Id: 1);

        _ = await Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Plugins;
using ProjectMetadataPlatform.Api.Plugins.Models;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Api.Tests.Plugins;

public class Tests
{
    private PluginsController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new PluginsController(_mediator.Object);
    }

    [Test]
    public async Task CreatePlugin_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<CreatePluginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        var request = new CreatePluginRequest(
            "Solid Rocket Booster",
            true,
            new List<string>(),
            "https://booster.de"
        );

        var result = await _controller.Put(request);

        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = result.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Value, Is.InstanceOf<CreatePluginResponse>());

        var pluginResponse = createdResult.Value as CreatePluginResponse;
        Assert.That(pluginResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(pluginResponse.Id, Is.EqualTo(42));
            Assert.That(createdResult.Location, Is.EqualTo("/Plugins/42"));
        });
    }

    [Test]
    public void CreatePlugin_WithError_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<CreatePluginCommand>(), It.IsAny<CancellationToken>()))
            .Throws(new IOException());

        var request = new CreatePluginRequest("Drogue chute", false, [], "https://chute.de");

        Assert.ThrowsAsync<IOException>(() => _controller.Put(request));
    }

    [Test]
    public void CreatePlugin_WithNameConflict_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<CreatePluginCommand>(), It.IsAny<CancellationToken>()))
            .Throws(new PluginNameAlreadyExistsException("Drogue chute"));

        var request = new CreatePluginRequest("Drogue chute", false, [], "https://chute.de");

        Assert.ThrowsAsync<PluginNameAlreadyExistsException>(() => _controller.Put(request));
    }

    [Test]
    public async Task CreatePlugin_EmptyName_Test()
    {
        var request = new CreatePluginRequest("", false, new List<string>(), "https://empty.de");

        var result = await _controller.Put(request);

        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var statusResult = result.Result as ObjectResult;

        Assert.That(statusResult, Is.Not.Null);

        Assert.That(statusResult.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public void DeletePlugin_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<DeleteGlobalPluginCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Delete(1));
    }

    [Test]
    public void PatchPlugin_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<PatchGlobalPluginCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        var exception = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.Patch(1, new PatchGlobalPluginRequest())
        );
        Assert.That(exception.Message, Is.EqualTo("An error message"));
    }

    [Test]
    public void PatchPlugin_WithNameConflictThrowsException_Test()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<PatchGlobalPluginCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new PluginNameAlreadyExistsException("Ariane 4"));
        var exception = Assert.ThrowsAsync<PluginNameAlreadyExistsException>(() =>
            _controller.Patch(1, new PatchGlobalPluginRequest("Ariane 4"))
        );
        Assert.That(
            exception.Message,
            Is.EqualTo("A global Plugin with the name Ariane 4 already exists.")
        );
    }

    [Test]
    public void GetGlobalPlugins_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetGlobalPluginsQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.GetGlobal());
    }

    [Test]
    public async Task CreatePlugin_WhiteSpacesName_Test()
    {
        var request = new CreatePluginRequest(
            "         ",
            false,
            new List<string>(),
            "https://whitespace.de"
        );

        var result = await _controller.Put(request);

        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var statusResult = result.Result as ObjectResult;

        Assert.That(statusResult, Is.Not.Null);

        Assert.That(statusResult.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task UpdateGlobalPlugin_Test()
    {
        var plugin = new Plugin
        {
            Id = 1,
            PluginName = "horn ox",
            IsArchived = true,
            BaseUrl = "https://hornox.com",
        };
        _mediator
            .Setup(m => m.Send(It.IsAny<PatchGlobalPluginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plugin);

        var request = new PatchGlobalPluginRequest(null, true);

        var result = await _controller.Patch(1, request);
        var okResult = result.Result as OkObjectResult;
        var resultValue = okResult?.Value as GetGlobalPluginResponse;

        Assert.Multiple(() =>
        {
            Assert.That(resultValue, Is.Not.Null);
            Assert.That(resultValue!.PluginName, Is.EqualTo("horn ox"));
            Assert.That(resultValue.IsArchived, Is.EqualTo(true));
            Assert.That(resultValue.Id, Is.EqualTo(1));
            Assert.That(resultValue.BaseUrl, Is.EqualTo("https://hornox.com"));
        });
    }

    [Test]
    public void Patch_PluginNotFound_ThrowsNotFound()
    {
        // Arrange
        const int pluginId = 1;
        var request = new PatchGlobalPluginRequest(null, true);

        _mediator
            .Setup(m => m.Send(It.IsAny<PatchGlobalPluginCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PluginNotFoundException(1));

        // Act
        Assert.ThrowsAsync<PluginNotFoundException>(() => _controller.Patch(pluginId, request));
    }

    [Test]
    public async Task GetGlobalPlugins_EmptyResponseList_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetGlobalPluginsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Plugin>());
        var result = await _controller.GetGlobal();

        Assert.That(result, Is.Not.Null);

        var value = result.Result as OkObjectResult;
        var responses = value?.Value as IEnumerable<GetGlobalPluginResponse>;

        Assert.That(responses, Is.Not.Null.And.Empty);
    }

    [Test]
    public async Task GetGlobalPlugins_Test()
    {
        var plugin = new Plugin
        {
            Id = 1,
            PluginName = "plugin 1",
            IsArchived = false,
            BaseUrl = "https://plugin1.com",
        };
        var pluginList = new List<Plugin> { plugin };

        _mediator
            .Setup(m => m.Send(It.IsAny<GetGlobalPluginsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pluginList);
        var result = await _controller.GetGlobal();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetGlobalPluginResponse>>());
        });

        var resultValue = (okResult?.Value as IEnumerable<GetGlobalPluginResponse>)!.ToList();
        Assert.That(resultValue, Has.Count.EqualTo(1));

        var resultObj = resultValue[0];
        Assert.Multiple(() =>
        {
            Assert.That(resultObj.PluginName, Is.EqualTo("plugin 1"));
            Assert.That(resultObj.Id, Is.EqualTo(1));
            Assert.That(resultObj.IsArchived, Is.False);
            Assert.That(resultObj.Keys, Is.EqualTo(Array.Empty<string>()));
            Assert.That(resultObj.BaseUrl, Is.EqualTo("https://plugin1.com"));
        });
    }

    [Test]
    public async Task DeleteGlobalPlugin_Test()
    {
        _mediator
            .Setup(m =>
                m.Send(It.IsAny<DeleteGlobalPluginCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        var result = await _controller.Delete(37);

        var okResult = result.Result as OkObjectResult;
        var resultValue = okResult?.Value as DeleteGlobalPluginResponse;

        Assert.Multiple(() =>
        {
            Assert.That(resultValue, Is.Not.Null);
            Assert.That(resultValue!.PluginId, Is.EqualTo(37));
        });
    }

    [Test]
    public void DeleteGlobalPlugin_PluginNotFound_Test()
    {
        _mediator
            .Setup(m =>
                m.Send(It.IsAny<DeleteGlobalPluginCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new PluginNotFoundException(1));

        Assert.ThrowsAsync<PluginNotFoundException>(() => _controller.Delete(1));
    }

    [Test]
    public async Task DeleteGlobalPlugin_InvalidId_Test()
    {
        var result = await _controller.Delete(0);

        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());

        var badRequestResult = result.Result as ObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(
                (badRequestResult?.Value as ErrorResponse)!.Message,
                Is.EqualTo("PluginId can't be smaller than or equal to 0")
            );
        });
    }
}

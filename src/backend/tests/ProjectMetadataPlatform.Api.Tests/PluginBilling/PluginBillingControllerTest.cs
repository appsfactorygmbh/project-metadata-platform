using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.PluginBilling;
using ProjectMetadataPlatform.Api.PluginBilling.Models;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.PluginBilling;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Api.Tests.PluginBilling;

[TestFixture]
public class PluginBillingControllerTest
{
    private PluginBillingController _controller;
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

        _controller = new PluginBillingController(_mediator.Object);
    }

    [Test]
    public async Task GetPluginBilling_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<
                    GetPluginBillingQuery,
                    (Domain.Billing.PluginBilling, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetPluginBillingQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.GetPluginBilling(0, 0));
    }

    [Test]
    public async Task GetPluginBilling_ResponseTest()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetPluginBillingQuery,
                    (Domain.Billing.PluginBilling, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetPluginBillingQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                (
                    new Domain.Billing.PluginBilling
                    {
                        ProjectId = 1,
                        PluginId = 1,
                        BillingId = 1,
                        DisplayName = "PluginBilling",
                        Currency = "",
                        BudgetLimit = 1,
                        HostingFee = 1,
                        TargetMargin = 0,
                        TimeFrame = TimeFrame.NEVER,
                        Date = null,
                        Notes = null,
                    },
                    []
                )
            );
        var result = await _controller.GetPluginBilling(1, 1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetPluginBillingResponse>());

        var getPluginBillingResponse = okResult.Value as GetPluginBillingResponse;
        Assert.That(getPluginBillingResponse, Is.Not.Null);

        Assert.That(getPluginBillingResponse.DisplayName, Is.EqualTo("PluginBilling"));
        Assert.That(getPluginBillingResponse.ProjectId, Is.EqualTo(1));
        Assert.That(getPluginBillingResponse.PluginId, Is.EqualTo(1));
        Assert.That(getPluginBillingResponse.TimeFrame, Is.EqualTo(TimeFrame.NEVER));
    }

    [Test]
    public async Task GetPluginBillingBySlug_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<
                    GetPluginBillingQuery,
                    (Domain.Billing.PluginBilling, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetPluginBillingQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.GetPluginBillingBySlug("", 0)
        );
    }

    [Test]
    public async Task GetPluginBillingBySlug_ResponseTest()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetPluginBillingQuery,
                    (Domain.Billing.PluginBilling, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetPluginBillingQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                (
                    new Domain.Billing.PluginBilling
                    {
                        ProjectId = 1,
                        PluginId = 1,
                        BillingId = 1,
                        DisplayName = "PluginBilling",
                        Currency = "",
                        BudgetLimit = 1,
                        HostingFee = 1,
                        TargetMargin = 0,
                        TimeFrame = TimeFrame.NEVER,
                        Date = null,
                        Notes = null,
                    },
                    []
                )
            );
        var result = await _controller.GetPluginBillingBySlug("Slug", 1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetPluginBillingResponse>());

        var getPluginBillingResponse = okResult.Value as GetPluginBillingResponse;
        Assert.That(getPluginBillingResponse, Is.Not.Null);

        Assert.That(getPluginBillingResponse.DisplayName, Is.EqualTo("PluginBilling"));
        Assert.That(getPluginBillingResponse.ProjectId, Is.EqualTo(1));
        Assert.That(getPluginBillingResponse.PluginId, Is.EqualTo(1));
        Assert.That(getPluginBillingResponse.TimeFrame, Is.EqualTo(TimeFrame.NEVER));
    }

    [Test]
    public async Task AddPluginBilling_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<AddPluginBillingCommand, (int, int)>(
                    It.IsAny<AddPluginBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.AddPluginBilling(
                new AddPluginBillingRequest(1, null, "", 1, 1, 1, TimeFrame.NEVER, null, null),
                1,
                1
            )
        );
    }

    [Test]
    public async Task AddPluginBilling_ReturnsIdTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<AddPluginBillingCommand, (int, int)>(
                    It.IsAny<AddPluginBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((1, 1));

        var result = await _controller.AddPluginBilling(
            new AddPluginBillingRequest(1, null, "", 1, 1, 1, TimeFrame.NEVER, null, null),
            1,
            1
        );
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Location, Is.EqualTo("/Projects/1/plugins/1/billing"));
        Assert.That(createdResult.Value, Is.InstanceOf<AddPluginBillingResponse>());

        var createPluginBillingResponse = createdResult.Value as AddPluginBillingResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createPluginBillingResponse, Is.Not.Null);
            Assert.That(createPluginBillingResponse!.PluginId, Is.EqualTo(1));
            Assert.That(createPluginBillingResponse!.ProjectId, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task AddPluginBillingBySlug_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<AddPluginBillingCommand, (int, int)>(
                    It.IsAny<AddPluginBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.AddPluginBillingBySlug(
                new AddPluginBillingRequest(1, null, "", 1, 1, 1, TimeFrame.NEVER, null, null),
                "Slug",
                1
            )
        );
    }

    [Test]
    public async Task AddPluginBillingBySlug_ReturnsIdTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<AddPluginBillingCommand, (int, int)>(
                    It.IsAny<AddPluginBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((1, 1));

        var result = await _controller.AddPluginBillingBySlug(
            new AddPluginBillingRequest(1, null, "", 1, 1, 1, TimeFrame.NEVER, null, null),
            "Slug",
            1
        );
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Location, Is.EqualTo("/Projects/Slug/plugins/1/billing"));
        Assert.That(createdResult.Value, Is.InstanceOf<AddPluginBillingResponse>());

        var createPluginBillingResponse = createdResult.Value as AddPluginBillingResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createPluginBillingResponse, Is.Not.Null);
            Assert.That(createPluginBillingResponse!.PluginId, Is.EqualTo(1));
            Assert.That(createPluginBillingResponse!.ProjectId, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task UpdatePluginBilling_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<UpdatePluginBillingCommand, Domain.Billing.PluginBilling>(
                    It.IsAny<UpdatePluginBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.UpdatePluginBilling(
                new UpdatePluginBillingRequest(null, "", 1, 1, 1, TimeFrame.NEVER, null, null),
                1,
                1
            )
        );
    }

    [Test]
    public async Task UpdatePluginBilling_ReturnsUpdatedPluginBillingTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<UpdatePluginBillingCommand, Domain.Billing.PluginBilling>(
                    It.IsAny<UpdatePluginBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Domain.Billing.PluginBilling
                {
                    Currency = "",
                    BudgetLimit = 1,
                    HostingFee = 1,
                    TargetMargin = 0,
                    TimeFrame = TimeFrame.NEVER,
                    BillingId = 1,
                    Date = null,
                    DisplayName = "PluginBilling",
                    PluginId = 1,
                    ProjectId = 1,
                    Notes = null,
                }
            );
        var result = await _controller.UpdatePluginBilling(
            new UpdatePluginBillingRequest(null, "", 1, 1, 1, TimeFrame.NEVER, null, null),
            1,
            1
        );
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetPluginBillingResponse>());

        var updatePluginBillingResponse = okResult.Value as GetPluginBillingResponse;

        Assert.Multiple(() =>
        {
            Assert.That(updatePluginBillingResponse, Is.Not.Null);
            Assert.That(updatePluginBillingResponse!.DisplayName, Is.EqualTo("PluginBilling"));
            Assert.That(updatePluginBillingResponse.ProjectId, Is.EqualTo(1));
            Assert.That(updatePluginBillingResponse.PluginId, Is.EqualTo(1));
            Assert.That(updatePluginBillingResponse.TimeFrame, Is.EqualTo(TimeFrame.NEVER));
        });
    }

    [Test]
    public async Task UpdatePluginBillingBySlug_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<UpdatePluginBillingCommand, Domain.Billing.PluginBilling>(
                    It.IsAny<UpdatePluginBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.UpdatePluginBillingBySlug(
                new UpdatePluginBillingRequest(null, "", 1, 1, 1, TimeFrame.NEVER, null, null),
                "Slug",
                1
            )
        );
    }

    [Test]
    public async Task UpdatePluginBillingBySlug_ReturnsUpdatedPluginBillingTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<UpdatePluginBillingCommand, Domain.Billing.PluginBilling>(
                    It.IsAny<UpdatePluginBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Domain.Billing.PluginBilling
                {
                    Currency = "",
                    BudgetLimit = 1,
                    HostingFee = 1,
                    TargetMargin = 0,
                    TimeFrame = TimeFrame.NEVER,
                    BillingId = 1,
                    Date = null,
                    DisplayName = "PluginBilling",
                    PluginId = 1,
                    ProjectId = 1,
                    Notes = null,
                }
            );
        var result = await _controller.UpdatePluginBillingBySlug(
            new UpdatePluginBillingRequest(null, "", 1, 1, 1, TimeFrame.NEVER, null, null),
            "Slug",
            1
        );
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetPluginBillingResponse>());

        var updatePluginBillingResponse = okResult.Value as GetPluginBillingResponse;

        Assert.Multiple(() =>
        {
            Assert.That(updatePluginBillingResponse, Is.Not.Null);
            Assert.That(updatePluginBillingResponse!.DisplayName, Is.EqualTo("PluginBilling"));
            Assert.That(updatePluginBillingResponse.ProjectId, Is.EqualTo(1));
            Assert.That(updatePluginBillingResponse.PluginId, Is.EqualTo(1));
            Assert.That(updatePluginBillingResponse.TimeFrame, Is.EqualTo(TimeFrame.NEVER));
        });
    }

    [Test]
    public async Task DeletePluginBilling_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<DeletePluginBillingCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.DeletePluginBilling(1, 1));
    }

    [Test]
    public async Task DeletePluginBilling_NoContentResponseTest()
    {
        var result = await _controller.DeletePluginBilling(1, 1);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task DeletePluginBillingBySlug_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<DeletePluginBillingCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.DeletePluginBillingBySlug("Slug", 1)
        );
    }

    [Test]
    public async Task DeletePluginBillingBySlug_NoContentResponseTest()
    {
        var result = await _controller.DeletePluginBillingBySlug("Slug", 1);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}

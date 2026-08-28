using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Billing;
using ProjectMetadataPlatform.Api.Billing.Models;
using ProjectMetadataPlatform.Api.Common.Models;
using ProjectMetadataPlatform.Application.Billing;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Api.Tests.Billing;

[TestFixture]
public class BillingControllerTest
{
    private BillingController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new BillingController(_mediator.Object);
    }

    [Test]
    public async Task GetBilling_EmptyResponseTest()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllBillingQuery,
                    (IEnumerable<GlobalBilling>, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetAllBillingQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(([], []));
        var result = await _controller.Get();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetBillingResponse>>());

        var getBillingResponseList = (
            okResult.Value as GetListResponse<GetBillingResponse>
        )!.Resources.ToList();
        Assert.That(getBillingResponseList, Is.Not.Null);

        Assert.That(getBillingResponseList, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetBilling_ListResponse()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetAllBillingQuery,
                    (IEnumerable<GlobalBilling>, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetAllBillingQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                (
                    [
                        new GlobalBilling { Id = 1, BillingKind = "Billing1" },
                        new GlobalBilling { Id = 2, BillingKind = "Billing2" },
                    ],
                    []
                )
            );
        var result = await _controller.Get();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetListResponse<GetBillingResponse>>());

        var getBillingResponseList = (
            okResult.Value as GetListResponse<GetBillingResponse>
        )!.Resources.ToList();
        Assert.That(getBillingResponseList, Is.Not.Null);

        Assert.That(getBillingResponseList, Has.Count.EqualTo(2));
        Assert.That(getBillingResponseList[0].Id, Is.EqualTo(1));
        Assert.That(getBillingResponseList[0].BillingKind, Is.EqualTo("Billing1"));
        Assert.That(getBillingResponseList[1].Id, Is.EqualTo(2));
        Assert.That(getBillingResponseList[1].BillingKind, Is.EqualTo("Billing2"));
    }

    [Test]
    public async Task GetBilling_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<
                    GetAllBillingQuery,
                    (IEnumerable<GlobalBilling>, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetAllBillingQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get());
    }

    [Test]
    public async Task GetBillingById_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<
                    GetBillingByIdQuery,
                    (GlobalBilling, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetBillingByIdQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get(0));
    }

    [Test]
    public async Task GetBilling_ResponseTest()
    {
        _ = _mediator
            .Setup(m =>
                m.Send<
                    GetBillingByIdQuery,
                    (GlobalBilling, IEnumerable<AuthorizationConstants.Actions>)
                >(It.IsAny<GetBillingByIdQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((new GlobalBilling { BillingKind = "Billing", Id = 1 }, []));
        var result = await _controller.Get(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetBillingResponse>());

        var getBillingResponse = okResult.Value as GetBillingResponse;
        Assert.That(getBillingResponse, Is.Not.Null);

        Assert.That(getBillingResponse.BillingKind, Is.EqualTo("Billing"));
    }

    [Test]
    public async Task PutBilling_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<CreateBillingCommand, int>(
                    It.IsAny<CreateBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.Put(new CreateBillingRequest("a", null, null, null, null, null))
        );
    }

    [Test]
    public async Task PutBilling_WhiteSpaceKind_BadRequestTest()
    {
        var result = await _controller.Put(
            new CreateBillingRequest("", null, null, null, null, null)
        );
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task PutBilling_ReturnsIdTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<CreateBillingCommand, int>(
                    It.IsAny<CreateBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        var request = new CreateBillingRequest("Billing", null, null, null, null, null);
        var result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Location, Is.EqualTo("/Billing/1"));
        Assert.That(createdResult.Value, Is.InstanceOf<CreateBillingResponse>());

        var createBillingResponse = createdResult.Value as CreateBillingResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createBillingResponse, Is.Not.Null);
            Assert.That(createBillingResponse!.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task UpdateBilling_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<UpdateBillingCommand, GlobalBilling>(
                    It.IsAny<UpdateBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.Update(1, new UpdateBillingRequest("Billing", null, null, null, null, null))
        );
    }

    [Test]
    public async Task UpdateBilling_WhiteSpaceKind_BadRequestTest()
    {
        var result = await _controller.Update(
            1,
            new UpdateBillingRequest("", null, null, null, null, null)
        );
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateBilling_ReturnsUpdatedBillingTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send<UpdateBillingCommand, GlobalBilling>(
                    It.IsAny<UpdateBillingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new GlobalBilling { BillingKind = "Billing", Id = 1 });
        var result = await _controller.Update(
            1,
            new UpdateBillingRequest("Billing", null, null, null, null, null)
        );
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetBillingResponse>());

        var updateBillingResponse = okResult.Value as GetBillingResponse;

        Assert.Multiple(() =>
        {
            Assert.That(updateBillingResponse, Is.Not.Null);
            Assert.That(updateBillingResponse?.BillingKind, Is.EqualTo("Billing"));
            Assert.That(updateBillingResponse!.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task DeleteBilling_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<DeleteBillingCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.Delete(1));
    }

    [Test]
    public async Task DeleteBilling_NoContentResponseTest()
    {
        var result = await _controller.Delete(1);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}

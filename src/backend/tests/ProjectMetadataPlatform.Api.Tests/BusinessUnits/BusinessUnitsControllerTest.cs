using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.BusinessUnits;
using ProjectMetadataPlatform.Api.BusinessUnits.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Application.BusinessUnits;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;

namespace ProjectMetadataPlatform.Api.Tests.BusinessUnits;

public class BusinessUnitsControllerTest
{
    private BusinessUnitsController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new BusinessUnitsController(_mediator.Object);
    }

    [Test]
    public async Task GetBusinessUnits_EmptyResponseTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllBusinessUnitsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var result = await _controller.Get();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetBusinessUnitResponse>>());

        var getBusinessUnitsResponseList = (
            okResult.Value as IEnumerable<GetBusinessUnitResponse>
        )!.ToList();
        Assert.That(getBusinessUnitsResponseList, Is.Not.Null);

        Assert.That(getBusinessUnitsResponseList, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetBusinessUnits_ListResponse()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllBusinessUnitsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new BusinessUnit { Id = 1, BusinessUnitName = "BusinessUnit1" },
                new BusinessUnit { Id = 2, BusinessUnitName = "BusinessUnit2" },
            ]);
        var result = await _controller.Get();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetBusinessUnitResponse>>());

        var getBusinessUnitsResponseList = (
            okResult.Value as IEnumerable<GetBusinessUnitResponse>
        )!.ToList();
        Assert.That(getBusinessUnitsResponseList, Is.Not.Null);

        Assert.That(getBusinessUnitsResponseList, Has.Count.EqualTo(2));
        Assert.That(getBusinessUnitsResponseList[0].Id, Is.EqualTo(1));
        Assert.That(getBusinessUnitsResponseList[0].BusinessUnitName, Is.EqualTo("BusinessUnit1"));
        Assert.That(getBusinessUnitsResponseList[1].Id, Is.EqualTo(2));
        Assert.That(getBusinessUnitsResponseList[1].BusinessUnitName, Is.EqualTo("BusinessUnit2"));
    }

    [Test]
    public async Task GetBusinessUnits_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetAllBusinessUnitsQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get());
    }

    [Test]
    public async Task GetBusinessUnit_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetBusinessUnitQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get(0));
    }

    [Test]
    public async Task GetBusinessUnit_ResponseTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetBusinessUnitQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BusinessUnit { BusinessUnitName = "BusinessUnit", Id = 1 });
        var result = await _controller.Get(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetBusinessUnitResponse>());

        var getTokenResponse = okResult.Value as GetBusinessUnitResponse;
        Assert.That(getTokenResponse, Is.Not.Null);

        Assert.That(getTokenResponse.BusinessUnitName, Is.EqualTo("BusinessUnit"));
    }

    [Test]
    public async Task PutBusinessUnit_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateBusinessUnitCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.Put(new CreateBusinessUnitRequest("a"))
        );
    }

    [Test]
    public async Task PutBusinessUnit_WhiteSpaceName_BadRequestTest()
    {
        var result = await _controller.Put(new CreateBusinessUnitRequest(""));
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task PutBusinessUnit_ReturnsIdTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateBusinessUnitCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);

        var request = new CreateBusinessUnitRequest("BusinessUnit");
        var result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Location, Is.EqualTo("BusinessUnits/1"));
        Assert.That(createdResult.Value, Is.InstanceOf<CreateBusinessUnitResponse>());

        var createBusinessUnitResponse = createdResult.Value as CreateBusinessUnitResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createBusinessUnitResponse, Is.Not.Null);
            Assert.That(createBusinessUnitResponse!.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task UpdateBusinessUnit_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<UpdateBusinessUnitCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.Patch(1, new UpdateBusinessUnitRequest())
        );
    }

    [Test]
    public async Task UpdateBusinessUnit_WhiteSpaceName_BadRequestTest()
    {
        var result = await _controller.Patch(1, new UpdateBusinessUnitRequest(""));
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateBusinessUnit_ReturnsUpdatedBusinessUnitTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<UpdateBusinessUnitCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new BusinessUnit { BusinessUnitName = "BusinessUnit", Id = 1 });
        var result = await _controller.Patch(1, new UpdateBusinessUnitRequest());
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetBusinessUnitResponse>());

        var updateBusinessUnitResponse = okResult.Value as GetBusinessUnitResponse;

        Assert.Multiple(() =>
        {
            Assert.That(updateBusinessUnitResponse, Is.Not.Null);
            Assert.That(updateBusinessUnitResponse.BusinessUnitName, Is.EqualTo("BusinessUnit"));
            Assert.That(updateBusinessUnitResponse.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task DeleteBusinessUnit_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<DeleteBusinessUnitCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Delete(1));
    }

    [Test]
    public async Task DeleteBusinessUnit_NoContentResponseTest()
    {
        var result = await _controller.Delete(1);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}

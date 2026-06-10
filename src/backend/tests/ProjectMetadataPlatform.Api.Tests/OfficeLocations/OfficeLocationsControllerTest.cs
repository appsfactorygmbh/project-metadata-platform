using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.OfficeLocations;
using ProjectMetadataPlatform.Api.OfficeLocations.Models;
using ProjectMetadataPlatform.Application.OfficeLocations;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Api.Tests.OfficeLocations;

public class OfficeLocationsControllerTest
{
    private OfficeLocationsController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new OfficeLocationsController(_mediator.Object);
    }

    [Test]
    public async Task GetOfficeLocations_EmptyResponseTest()
    {
        _ = _mediator
            .Setup(m =>
                m.Send(It.IsAny<GetAllOfficeLocationsQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        var result = await _controller.Get();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetOfficeLocationResponse>>());

        var getOfficeLocationsResponseList = (
            okResult.Value as IEnumerable<GetOfficeLocationResponse>
        )!.ToList();
        Assert.That(getOfficeLocationsResponseList, Is.Not.Null);

        Assert.That(getOfficeLocationsResponseList, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetOfficeLocations_ListResponse()
    {
        _ = _mediator
            .Setup(m =>
                m.Send(It.IsAny<GetAllOfficeLocationsQuery>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([
                new OfficeLocation { Id = 1, OfficeLocationName = "OfficeLocation1" },
                new OfficeLocation { Id = 2, OfficeLocationName = "OfficeLocation2" },
            ]);
        var result = await _controller.Get();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetOfficeLocationResponse>>());

        var getOfficeLocationsResponseList = (
            okResult.Value as IEnumerable<GetOfficeLocationResponse>
        )!.ToList();
        Assert.That(getOfficeLocationsResponseList, Is.Not.Null);

        Assert.That(getOfficeLocationsResponseList, Has.Count.EqualTo(2));
        Assert.That(getOfficeLocationsResponseList[0].Id, Is.EqualTo(1));
        Assert.That(
            getOfficeLocationsResponseList[0].OfficeLocationName,
            Is.EqualTo("OfficeLocation1")
        );
        Assert.That(getOfficeLocationsResponseList[1].Id, Is.EqualTo(2));
        Assert.That(
            getOfficeLocationsResponseList[1].OfficeLocationName,
            Is.EqualTo("OfficeLocation2")
        );
    }

    [Test]
    public async Task GetOfficeLocations_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetAllOfficeLocationsQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get());
    }

    [Test]
    public async Task GetOfficeLocation_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetOfficeLocationQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get(0));
    }

    [Test]
    public async Task GetOfficeLocation_ResponseTest()
    {
        _ = _mediator
            .Setup(m => m.Send(It.IsAny<GetOfficeLocationQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OfficeLocation { OfficeLocationName = "OfficeLocation", Id = 1 });
        var result = await _controller.Get(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetOfficeLocationResponse>());

        var getDepartmentResponse = okResult.Value as GetOfficeLocationResponse;
        Assert.That(getDepartmentResponse, Is.Not.Null);

        Assert.That(getDepartmentResponse.OfficeLocationName, Is.EqualTo("OfficeLocation"));
    }

    [Test]
    public async Task PutOfficeLocation_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(
                    It.IsAny<CreateOfficeLocationCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.Put(new CreateOfficeLocationRequest("a"))
        );
    }

    [Test]
    public async Task PutOfficeLocation_WhiteSpaceName_BadRequestTest()
    {
        var result = await _controller.Put(new CreateOfficeLocationRequest(""));
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task PutOfficeLocation_ReturnsIdTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(
                    It.IsAny<CreateOfficeLocationCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        var request = new CreateOfficeLocationRequest("OfficeLocation");
        var result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Location, Is.EqualTo("OfficeLocations/1"));
        Assert.That(createdResult.Value, Is.InstanceOf<CreateOfficeLocationResponse>());

        var createOfficeLocationResponse = createdResult.Value as CreateOfficeLocationResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createOfficeLocationResponse, Is.Not.Null);
            Assert.That(createOfficeLocationResponse!.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task UpdateOfficeLocation_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(
                    It.IsAny<UpdateOfficeLocationCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.Patch(1, new UpdateOfficeLocationRequest())
        );
    }

    [Test]
    public async Task UpdateOfficeLocation_WhiteSpaceName_BadRequestTest()
    {
        var result = await _controller.Patch(1, new UpdateOfficeLocationRequest(""));
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateOfficeLocation_ReturnsUpdatedOfficeLocationTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(
                    It.IsAny<UpdateOfficeLocationCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new OfficeLocation { OfficeLocationName = "OfficeLocation", Id = 1 });
        var result = await _controller.Patch(1, new UpdateOfficeLocationRequest());
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetOfficeLocationResponse>());

        var updateOfficeLocationResponse = okResult.Value as GetOfficeLocationResponse;

        Assert.Multiple(() =>
        {
            Assert.That(updateOfficeLocationResponse, Is.Not.Null);
            Assert.That(
                updateOfficeLocationResponse.OfficeLocationName,
                Is.EqualTo("OfficeLocation")
            );
            Assert.That(updateOfficeLocationResponse.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task DeleteOfficeLocation_MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(
                    It.IsAny<DeleteOfficeLocationCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.Delete(1));
    }

    [Test]
    public async Task DeleteOfficeLocation_NoContentResponseTest()
    {
        var result = await _controller.Delete(1);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}

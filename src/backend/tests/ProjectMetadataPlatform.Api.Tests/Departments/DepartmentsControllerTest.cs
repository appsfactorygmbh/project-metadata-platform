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
using ProjectMetadataPlatform.Api.Departments;
using ProjectMetadataPlatform.Api.Departments.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Application.Departments;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;

namespace ProjectMetadataPlatform.Api.Tests.Departments;

public class DepartmentsControllerTest
{
    private DepartmentsController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new DepartmentsController(_mediator.Object);
    }

    [Test]
    public async Task GetDepartments_EmptyResponseTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllDepartmentsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var result = await _controller.Get();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetDepartmentResponse>>());

        var getDepartmentsResponseList = (
            okResult.Value as IEnumerable<GetDepartmentResponse>
        )!.ToList();
        Assert.That(getDepartmentsResponseList, Is.Not.Null);

        Assert.That(getDepartmentsResponseList, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetDepartments_ListResponse()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllDepartmentsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Department { Id = 1, DepartmentName = "Department1" },
                new Department { Id = 2, DepartmentName = "Department2" },
            ]);
        var result = await _controller.Get();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetDepartmentResponse>>());

        var getDepartmentsResponseList = (
            okResult.Value as IEnumerable<GetDepartmentResponse>
        )!.ToList();
        Assert.That(getDepartmentsResponseList, Is.Not.Null);

        Assert.That(getDepartmentsResponseList, Has.Count.EqualTo(2));
        Assert.That(getDepartmentsResponseList[0].Id, Is.EqualTo(1));
        Assert.That(getDepartmentsResponseList[0].DepartmentName, Is.EqualTo("Department1"));
        Assert.That(getDepartmentsResponseList[1].Id, Is.EqualTo(2));
        Assert.That(getDepartmentsResponseList[1].DepartmentName, Is.EqualTo("Department2"));
    }

    [Test]
    public async Task GetDepartments_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetAllDepartmentsQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get());
    }

    [Test]
    public async Task GetDepartment_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetDepartmentQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get(0));
    }

    [Test]
    public async Task GetDepartment_ResponseTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetDepartmentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Department { DepartmentName = "Department", Id = 1 });
        var result = await _controller.Get(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetDepartmentResponse>());

        var getDepartmentResponse = okResult.Value as GetDepartmentResponse;
        Assert.That(getDepartmentResponse, Is.Not.Null);

        Assert.That(getDepartmentResponse.DepartmentName, Is.EqualTo("Department"));
    }

    [Test]
    public async Task PutDepartment_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateDepartmentCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.Put(new CreateDepartmentRequest("a"))
        );
    }

    [Test]
    public async Task PutDepartment_WhiteSpaceName_BadRequestTest()
    {
        var result = await _controller.Put(new CreateDepartmentRequest(""));
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task PutDepartment_ReturnsIdTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateDepartmentCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);

        var request = new CreateDepartmentRequest("Department");
        var result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Location, Is.EqualTo("Departments/1"));
        Assert.That(createdResult.Value, Is.InstanceOf<CreateDepartmentResponse>());

        var createDepartmentResponse = createdResult.Value as CreateDepartmentResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createDepartmentResponse, Is.Not.Null);
            Assert.That(createDepartmentResponse!.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task UpdateDepartment_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<UpdateDepartmentCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.Patch(1, new UpdateDepartmentRequest())
        );
    }

    [Test]
    public async Task UpdateDepartment_WhiteSpaceName_BadRequestTest()
    {
        var result = await _controller.Patch(1, new UpdateDepartmentRequest(""));
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateDepartment_ReturnsUpdatedDepartmentTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<UpdateDepartmentCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Department { DepartmentName = "Department", Id = 1 });
        var result = await _controller.Patch(1, new UpdateDepartmentRequest());
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetDepartmentResponse>());

        var updateDepartmentResponse = okResult.Value as GetDepartmentResponse;

        Assert.Multiple(() =>
        {
            Assert.That(updateDepartmentResponse, Is.Not.Null);
            Assert.That(updateDepartmentResponse.DepartmentName, Is.EqualTo("Department"));
            Assert.That(updateDepartmentResponse.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task DeleteDepartment_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<DeleteDepartmentCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Delete(1));
    }

    [Test]
    public async Task DeleteDepartment_NoContentResponseTest()
    {
        var result = await _controller.Delete(1);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}

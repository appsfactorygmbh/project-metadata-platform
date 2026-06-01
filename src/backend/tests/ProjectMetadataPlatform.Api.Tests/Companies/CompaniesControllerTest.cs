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
using ProjectMetadataPlatform.Api.Companies;
using ProjectMetadataPlatform.Api.Companies.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Application.Companies;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;

namespace ProjectMetadataPlatform.Api.Tests.Companies;

public class CompaniesControllerTest
{
    private CompaniesController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new CompaniesController(_mediator.Object);
    }

    [Test]
    public async Task GetCompanies_EmptyResponseTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllCompaniesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var result = await _controller.Get();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetCompanyResponse>>());

        var getCompaniesResponseList = (
            okResult.Value as IEnumerable<GetCompanyResponse>
        )!.ToList();
        Assert.That(getCompaniesResponseList, Is.Not.Null);

        Assert.That(getCompaniesResponseList, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetCompanies_ListResponse()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetAllCompaniesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Company { Id = 1, CompanyName = "Company1" },
                new Company { Id = 2, CompanyName = "Company2" },
            ]);
        var result = await _controller.Get();
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<IEnumerable<GetCompanyResponse>>());

        var getCompaniesResponseList = (
            okResult.Value as IEnumerable<GetCompanyResponse>
        )!.ToList();
        Assert.That(getCompaniesResponseList, Is.Not.Null);

        Assert.That(getCompaniesResponseList, Has.Count.EqualTo(2));
        Assert.That(getCompaniesResponseList[0].Id, Is.EqualTo(1));
        Assert.That(getCompaniesResponseList[0].CompanyName, Is.EqualTo("Company1"));
        Assert.That(getCompaniesResponseList[1].Id, Is.EqualTo(2));
        Assert.That(getCompaniesResponseList[1].CompanyName, Is.EqualTo("Company2"));
    }

    [Test]
    public async Task GetCompanies_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetAllCompaniesQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get());
    }

    [Test]
    public async Task GetCompany_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetCompanyQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get(0));
    }

    [Test]
    public async Task GetCompany_ResponseTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetCompanyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Company { CompanyName = "Company", Id = 1 });
        var result = await _controller.Get(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetCompanyResponse>());

        var getCompanyResponse = okResult.Value as GetCompanyResponse;
        Assert.That(getCompanyResponse, Is.Not.Null);

        Assert.That(getCompanyResponse.CompanyName, Is.EqualTo("Company"));
    }

    [Test]
    public async Task GetLinkedProjects_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetLinkedProjectsQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.GetLinkedProjects(1));
    }

    [Test]
    public async Task GetLinkedProjects_ResponseTest()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<GetLinkedProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(["1", "2", "3"]);
        var result = await _controller.GetLinkedProjects(1);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetLinkedProjectsForCompanyResponse>());

        var getBuResponse = okResult.Value as GetLinkedProjectsForCompanyResponse;
        Assert.That(getBuResponse, Is.Not.Null);

        Assert.That(getBuResponse.ProjectSlugs, Is.EqualTo(new List<string> {"1", "2", "3" }));
    }

    [Test]
    public async Task PutCompany_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateCompanyCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.Put(new CreateCompanyRequest("a"))
        );
    }

    [Test]
    public async Task PutCompany_WhiteSpaceName_BadRequestTest()
    {
        var result = await _controller.Put(new CreateCompanyRequest(""));
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task PutCompany_ReturnsIdTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<CreateCompanyCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(1);

        var request = new CreateCompanyRequest("Company");
        var result = await _controller.Put(request);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.Location, Is.EqualTo("Companies/1"));
        Assert.That(createdResult.Value, Is.InstanceOf<CreateCompanyResponse>());

        var createCompanyResponse = createdResult.Value as CreateCompanyResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createCompanyResponse, Is.Not.Null);
            Assert.That(createCompanyResponse!.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task UpdateCompany_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<UpdateCompanyCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() =>
            _controller.Patch(1, new UpdateCompanyRequest())
        );
    }

    [Test]
    public async Task UpdateCompany_WhiteSpaceName_BadRequestTest()
    {
        var result = await _controller.Patch(1, new UpdateCompanyRequest(""));
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateCompany_ReturnsUpdatedCompanyTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<UpdateCompanyCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Company { CompanyName = "Company", Id = 1 });
        var result = await _controller.Patch(1, new UpdateCompanyRequest());
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetCompanyResponse>());

        var updateCompanyResponse = okResult.Value as GetCompanyResponse;

        Assert.Multiple(() =>
        {
            Assert.That(updateCompanyResponse, Is.Not.Null);
            Assert.That(updateCompanyResponse.CompanyName, Is.EqualTo("Company"));
            Assert.That(updateCompanyResponse.Id, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task DeleteCompany_MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<DeleteCompanyCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Delete(1));
    }

    [Test]
    public async Task DeleteCompany_NoContentResponseTest()
    {
        var result = await _controller.Delete(1);
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}

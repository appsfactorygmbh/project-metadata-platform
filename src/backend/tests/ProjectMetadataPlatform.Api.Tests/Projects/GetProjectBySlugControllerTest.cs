using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Projects;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Tests.Projects;

[TestFixture]
public class GetProjectBySlugControllerTest
{
    private ProjectsController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new ProjectsController(_mediator.Object);
    }

    [Test]
    public void MediatorThrowsExceptionTest()
    {
        _ = _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetProjectIdBySlugQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        _ = Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get("test"));
    }

    [Test]
    public async Task GetProjectTest()
    {
        // prepare
        var projectsResponseContent = new Project
        {
            Id = 50,
            ProjectName = "MetaDataPlatform",
            Slug = "metadataplatform",
            ClientName = "Appsfactory",
            OfferId = "1023",
            Company = new() { CompanyName = "Charlies Schokoladenfabrik" },
            CompanyId = 1,
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
        };
        _ = _mediator
            .Setup(m =>
                m.Send(
                    It.Is<GetProjectIdBySlugQuery>(q => q.Slug == "metadataplatform"),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(50);
        _ = _mediator
            .Setup(m =>
                m.Send(It.Is<GetProjectQuery>(q => q.Id == 50), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((projectsResponseContent, []));

        // act
        var result = await _controller.Get("metadataplatform");

        // assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetProjectDetailsResponse>());

        var project = okResult.Value as GetProjectDetailsResponse;
        Assert.That(project, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(project.Id, Is.EqualTo(50));
            Assert.That(project.ProjectName, Is.EqualTo("MetaDataPlatform"));
            Assert.That(project.Slug, Is.EqualTo("metadataplatform"));
            Assert.That(project.ClientName, Is.EqualTo("Appsfactory"));
            Assert.That(project.OfferId, Is.EqualTo("1023"));
            Assert.That(project.Company.CompanyName, Is.EqualTo("Charlies Schokoladenfabrik"));
            Assert.That(project.CompanyState, Is.EqualTo(CompanyState.EXTERNAL));
            Assert.That(project.IsmsLevel, Is.EqualTo(SecurityLevel.VERY_HIGH));
        });
    }
}

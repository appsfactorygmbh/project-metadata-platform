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
public class GetProjectByIdControllerTest
{
    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new ProjectsController(_mediator.Object);
    }

    private ProjectsController _controller;
    private Mock<IMediator> _mediator;

    [Test]
    public void MediatorThrowsExceptionTest()
    {
        _mediator
            .Setup(mediator =>
                mediator.Send(It.IsAny<GetProjectQuery>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Get(1));
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
            Company = "Charlies Schokoladenfabrik",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
        };
        _mediator
            .Setup(m =>
                m.Send(It.Is<GetProjectQuery>(q => q.Id == 50), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(projectsResponseContent);

        // act
        var result = await _controller.Get(50);

        // assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.InstanceOf<GetProjectResponse>());

        var project = okResult.Value as GetProjectResponse;
        Assert.That(project, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(project.Id, Is.EqualTo(50));
            Assert.That(project.ProjectName, Is.EqualTo("MetaDataPlatform"));
            Assert.That(project.Slug, Is.EqualTo("metadataplatform"));
            Assert.That(project.ClientName, Is.EqualTo("Appsfactory"));
            Assert.That(project.OfferId, Is.EqualTo("1023"));
            Assert.That(project.Company, Is.EqualTo("Charlies Schokoladenfabrik"));
            Assert.That(project.CompanyState, Is.EqualTo(CompanyState.EXTERNAL));
            Assert.That(project.IsmsLevel, Is.EqualTo(SecurityLevel.VERY_HIGH));
        });
    }
}

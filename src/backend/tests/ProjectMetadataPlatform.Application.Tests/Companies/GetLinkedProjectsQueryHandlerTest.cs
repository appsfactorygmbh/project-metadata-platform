using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Companies;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.Tests.Companies;

[TestFixture]
public class GetLinkedProjectsQueryHandlerTest
{
    private GetLinkedProjectsQueryHandler _handler;
    private Mock<ICompanyRepository> _mockCompanyRepository;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _handler = new GetLinkedProjectsQueryHandler(
            companyRepository: _mockCompanyRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetLinkedProjects_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnCompany = new Company()
        {
            Id = 1,
            CompanyName = "Test_1",
            Projects =
            [
                new()
                {
                    Id = 111,
                    ProjectName = "Projects",
                    Slug = "project_1",
                    ClientName = "Project Client",
                    CompanyId = 1,
                },
                new()
                {
                    Id = 222,
                    ProjectName = "Projects",
                    Slug = "project_2",
                    ClientName = "Project Client",
                    CompanyId = 1,
                },
            ],
        };

        _ = _mockCompanyRepository
            .Setup(repo => repo.GetCompanyWithProjectsAsync(It.IsAny<int>()))
            .ReturnsAsync(returnCompany);

        // Act
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Company>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        var result = await _handler.Handle(
            new GetLinkedProjectsQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result, Does.Contain("project_1"));
            Assert.That(result, Does.Contain("project_2"));
        });
        _mockCompanyRepository.Verify(
            m => m.GetCompanyWithProjectsAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
    }

    [Test]
    public async Task GetLinkedProjects_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Company>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);

        var request = new GetLinkedProjectsQuery(Id: 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

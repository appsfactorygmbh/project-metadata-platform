using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class GetAllProjectsQueryHandlerTest
{
    private GetAllProjectsQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IPaginationHelper> _paginationHelperMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelperMock = new Mock<IPaginationHelper>();
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetAllProjectsQueryHandler(
            _mockProjectRepo.Object,
            authorizationService: _authorizationServiceMock.Object,
            _paginationHelperMock.Object
        );
    }

    [Test]
    public async Task CallsRepositoryWithRequest()
    {
        _ = _mockProjectRepo
            .Setup(m =>
                m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>(), It.IsAny<ProjectCursor?>())
            )
            .ReturnsAsync(new List<Project> { }.BuildMock());

        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Project> query, int? limit) => (query, null));
        var request = new GetAllProjectsQuery(null, "", null, null);
        _ = await _handler.Handle(request, CancellationToken.None);

        _mockProjectRepo.Verify(
            repository => repository.GetProjectsAsync(request, null),
            Times.Once
        );
        _mockProjectRepo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ReturnsProjectsFromRepository()
    {
        var request = new GetAllProjectsQuery(null, "", null, null);
        var team = new Team()
        {
            TeamName = "AF_1",
            Id = 1,
            BusinessUnit = new() { BusinessUnitName = "Health" },
            BusinessUnitId = 1,
        };
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Project> query, int? limit) => (query, null));
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                ClientName = "Metatron",
                Team = team,
                TeamId = 1,
                Company = new() { CompanyName = "Ag der Ags" },
                CompanyId = 1,
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                ClientName = "Lucifer",
                Company = new() { CompanyName = "Ag der Ags" },
                CompanyId = 1,
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                ClientName = "Satan",

                Company = new() { CompanyName = "Ark" },
                CompanyId = 2,
                IsmsLevel = SecurityLevel.HIGH,
            },
        };

        _ = _mockProjectRepo
            .Setup(m =>
                m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>(), It.IsAny<ProjectCursor?>())
            )
            .ReturnsAsync(projects.BuildMock());
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result.Item1, Is.EquivalentTo(projects));
    }
}

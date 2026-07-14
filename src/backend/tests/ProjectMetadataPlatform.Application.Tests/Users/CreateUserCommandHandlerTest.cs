using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class CreateUserCommandHandlerTest
{
    private CreateUserCommandHandler _handler;
    private Mock<IUsersRepository> _mockUsersRepo;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<ITeamRepository> _mockTeamRepo;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IOfficeLocationRepository> _mockOfficeLocationRepository;

    private Mock<ICompanyRepository> _mockCompanyRepository;

    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;
    private Mock<IDepartmentRepository> _mockDepartmentRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockUsersRepo = new Mock<IUsersRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockTeamRepo = new Mock<ITeamRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _mockOfficeLocationRepository = new Mock<IOfficeLocationRepository>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _handler = new CreateUserCommandHandler(
            _mockUsersRepo.Object,
            _mockLogRepo.Object,
            _mockTeamRepo.Object,
            _mockDepartmentRepository.Object,
            _mockBusinessUnitRepository.Object,
            _mockOfficeLocationRepository.Object,
            _mockCompanyRepository.Object,
            _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task CreateUser_Test()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockTeamRepo
            .SetupSequence(m => m.GetTeamByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new Team
                {
                    TeamName = "Team1",
                    BusinessUnit = new() { BusinessUnitName = "BU Test" },
                    BusinessUnitId = 1,
                }
            )
            .ReturnsAsync(
                new Team
                {
                    TeamName = "Team2",
                    BusinessUnit = new() { BusinessUnitName = "BU Test" },
                    BusinessUnitId = 1,
                }
            );
        _ = _mockUnitOfWork.Setup(m => m.CompleteAsync()).Returns(Task.CompletedTask);
        _ = _mockLogRepo
            .Setup(m =>
                m.AddUserLogForCurrentActor(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                )
            )
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new CreateUserCommand(
                "Id",
                "Example Email",
                "Example Password",
                true,
                true,
                ["Team1"],
                ["Team2"],
                null,
                [],
                ["Design"],
                "Leipzig",
                "Appsfactory"
            ),
            It.IsAny<CancellationToken>()
        );
        Assert.Multiple(() =>
        {
            Assert.That(result.EmployeeId, Is.EqualTo("Id"));
            Assert.That(result.UserName, Is.EqualTo("Example Email"));
            Assert.That(result.Email, Is.EqualTo("Example Email"));
            Assert.That(result.IsActive, Is.EqualTo(true));
            Assert.That(result.IsScimProvisioned, Is.EqualTo(true));
            Assert.That(result.Teams?.FirstOrDefault()?.TeamName, Is.EqualTo("Team1"));
            Assert.That(result.TeamSupport?.FirstOrDefault()?.TeamName, Is.EqualTo("Team2"));
            Assert.That(result.BusinessUnits, Is.EqualTo(null));
            Assert.That(result.JobTitles, Is.EqualTo(null));
            Assert.That(result.Departments?.FirstOrDefault()?.DepartmentName, Is.EqualTo("Design"));
            Assert.That(result.OfficeLocation!.OfficeLocationName, Is.EqualTo("Leipzig"));
            Assert.That(result.Company!.CompanyName, Is.EqualTo("Appsfactory"));
        });
    }

    [Test]
    public void CreateUser_ThrowsException_Test()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockUsersRepo
            .Setup(m => m.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Error"));
        _ = _mockUnitOfWork.Setup(m => m.CompleteAsync()).Returns(Task.CompletedTask);
        _ = _mockLogRepo
            .Setup(m =>
                m.AddUserLogForCurrentActor(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                )
            )
            .Returns(Task.CompletedTask);

        _ = Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(
                new CreateUserCommand(
                    "Example Email",
                    "Example Password",
                    null,
                    true,
                    true,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task CreateUserLog_Test()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockUsersRepo
            .Setup(m => m.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync("1");
        _ = _mockUnitOfWork.Setup(m => m.CompleteAsync()).Returns(Task.CompletedTask);
        _ = await _handler.Handle(
            new CreateUserCommand(
                "",
                "thetruestrepairmanwillrepairmen@greendale.edu",
                null,
                true,
                true,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            ),
            It.IsAny<CancellationToken>()
        );

        _mockLogRepo.Verify(
            m =>
                m.AddUserLogForCurrentActor(
                    It.Is<ApplicationUser>(user =>
                        user.Email == "thetruestrepairmanwillrepairmen@greendale.edu"
                    ),
                    Action.ADDED_USER,
                    It.Is<List<LogChange>>(changes =>
                        changes.Any(change =>
                            change.Property == "Email"
                            && change.OldValue == ""
                            && change.NewValue == "thetruestrepairmanwillrepairmen@greendale.edu"
                        )
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task CreateUser_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);

        var request = new CreateUserCommand(
            "",
            "thetruestrepairmanwillrepairmen@greendale.edu",
            null,
            true,
            true,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

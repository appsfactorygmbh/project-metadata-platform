using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
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
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockUsersRepo = new Mock<IUsersRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockTeamRepo = new Mock<ITeamRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateUserCommandHandler(
            _mockUsersRepo.Object,
            _mockLogRepo.Object,
            _mockTeamRepo.Object,
            _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task CreateUser_Test()
    {
        _mockTeamRepo
            .SetupSequence(m => m.GetTeamByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new Team { TeamName = "Team1", BusinessUnit = "BU" })
            .ReturnsAsync(new Team { TeamName = "Team2", BusinessUnit = "BU" });
        _mockUnitOfWork.Setup(m => m.CompleteAsync()).Returns(Task.CompletedTask);
        _mockLogRepo
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
            Assert.That(result.Departments, Is.EqualTo(new List<string> { "Design" }));
            Assert.That(result.Company, Is.EqualTo("Appsfactory"));
        });
    }

    [Test]
    public void CreateUser_ThrowsException_Test()
    {
        _mockUsersRepo
            .Setup(m => m.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Error"));
        _mockUnitOfWork.Setup(m => m.CompleteAsync()).Returns(Task.CompletedTask);
        _mockLogRepo
            .Setup(m =>
                m.AddUserLogForCurrentActor(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                )
            )
            .Returns(Task.CompletedTask);

        Assert.ThrowsAsync<Exception>(() =>
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
                    null
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task CreateUserLog_Test()
    {
        _mockUsersRepo
            .Setup(m => m.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync("1");
        _mockUnitOfWork.Setup(m => m.CompleteAsync()).Returns(Task.CompletedTask);
        await _handler.Handle(
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
}

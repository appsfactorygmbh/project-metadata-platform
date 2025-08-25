using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class CreateUserCommandHandlerTest
{
    private CreateUserCommandHandler _handler;
    private Mock<IUsersRepository> _mockUsersRepo;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockUsersRepo = new Mock<IUsersRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateUserCommandHandler(
            _mockUsersRepo.Object,
            _mockLogRepo.Object,
            _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task CreateUser_Test()
    {
        _mockUsersRepo
            .Setup(m => m.CreateUserAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync("1");
        _mockUnitOfWork.Setup(m => m.CompleteAsync()).Returns(Task.CompletedTask);
        _mockLogRepo
            .Setup(m =>
                m.AddUserLogForCurrentUser(
                    It.IsAny<IdentityUser>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                )
            )
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new CreateUserCommand("Example Email", "Example Password"),
            It.IsAny<CancellationToken>()
        );

        Assert.That(result, Is.EqualTo("1"));
    }

    [Test]
    public void CreateUser_ThrowsException_Test()
    {
        _mockUsersRepo
            .Setup(m => m.CreateUserAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Error"));
        _mockUnitOfWork.Setup(m => m.CompleteAsync()).Returns(Task.CompletedTask);
        _mockLogRepo
            .Setup(m =>
                m.AddUserLogForCurrentUser(
                    It.IsAny<IdentityUser>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                )
            )
            .Returns(Task.CompletedTask);

        Assert.ThrowsAsync<Exception>(() =>
            _handler.Handle(
                new CreateUserCommand("Example Email", "Example Password"),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task CreateUserLog_Test()
    {
        _mockUsersRepo
            .Setup(m => m.CreateUserAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync("1");
        _mockUnitOfWork.Setup(m => m.CompleteAsync()).Returns(Task.CompletedTask);
        await _handler.Handle(
            new CreateUserCommand("thetruestrepairmanwillrepairmen@greendale.edu", ""),
            It.IsAny<CancellationToken>()
        );

        _mockLogRepo.Verify(
            m =>
                m.AddUserLogForCurrentUser(
                    It.Is<IdentityUser>(user =>
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

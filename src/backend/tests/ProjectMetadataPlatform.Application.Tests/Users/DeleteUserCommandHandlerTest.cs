using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.UserException;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Users;
using UserAction = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class DeleteUserCommandHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockUsersRepo = new Mock<IUsersRepository>();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Email, "camo")], "TestAuth");
        var contextUser = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = contextUser };
        _ = httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext)
            .Returns(httpContext);
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogRepository = new Mock<ILogRepository>();
        _ = _mockLogRepository
            .Setup(repository => repository.GetLogsWithSearch(It.IsAny<string>()))
            .ReturnsAsync(new List<Log> { }.AsQueryable());
        _handler = new DeleteUserCommandHandler(
            _mockUsersRepo.Object,
            httpContextAccessorMock.Object,
            _mockLogRepository.Object,
            _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
        _ = httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext.User)
            .Returns(contextUser);
    }

    private Mock<IAuthorizationService> _authorizationServiceMock;
    private DeleteUserCommandHandler _handler;
    private Mock<IUsersRepository> _mockUsersRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ILogRepository> _mockLogRepository;

    [Test]
    public async Task DeleteUser_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "2",
            Id = "1",
            Email = "user@example.com",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.DELETE, true },
                }
            );
        _ = _mockUsersRepo.Setup(m => m.GetUserByIdAsync("2")).ReturnsAsync(user);
        _ = _mockUsersRepo.Setup(m => m.DeleteUserAsync(user)).ReturnsAsync(user);
        var result = await _handler.Handle(new DeleteUserCommand("2"), CancellationToken.None);
        _mockLogRepository.Verify(
            m =>
                m.AddUserLogForCurrentActor(
                    It.Is<ApplicationUser>(u => u.Id == "1"),
                    UserAction.REMOVED_USER,
                    It.Is<List<LogChange>>(changes =>
                        changes.Count == 1
                        && changes[0].OldValue == "user@example.com"
                        && changes[0].NewValue == ""
                        && changes[0].Property == nameof(ApplicationUser.Email)
                    )
                ),
            Times.Once
        );
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public void DeleteUser_InvalidUser_Test()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.DELETE, true },
                }
            );
        _ = _mockUsersRepo
            .Setup(m => m.GetUserByIdAsync("1"))
            .ThrowsAsync(new UserNotFoundException("1"));

        _ = Assert.ThrowsAsync<UserNotFoundException>(() =>
            _handler.Handle(new DeleteUserCommand("1"), CancellationToken.None)
        );
    }

    [Test]
    public void DeleteUser_SelfDeletionAttempt_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "200",
            Email = "camo",
            Id = "1",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.DELETE, true },
                }
            );
        _ = _mockUsersRepo.Setup(m => m.GetUserByIdAsync("200")).ReturnsAsync(user);
        _ = _mockUsersRepo.Setup(m => m.GetUserByEmailAsync("camo")).ReturnsAsync(user);

        _ = Assert.ThrowsAsync<UserCantDeleteThemselfException>(() =>
            _handler.Handle(new DeleteUserCommand("200"), CancellationToken.None)
        );
    }

    [Test]
    public async Task DeleteUser_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.DELETE, false },
                }
            );

        var request = new DeleteUserCommand("200");

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

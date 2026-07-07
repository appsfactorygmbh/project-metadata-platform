using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class GetUserByEmailQueryHandlerTest
{
    private GetUserByEmailQueryHandler _handler;
    private Mock<IUsersRepository> _mockUserRepo;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockUserRepo = new Mock<IUsersRepository>();
        _handler = new GetUserByEmailQueryHandler(
            _mockUserRepo.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task HandleGetUserByEmail_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "abc",
            Id = "13",
            Email = "squidlauncher@bankofevil.com",
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
                    { AuthorizationConstants.Actions.GET, true },
                }
            );
        _ = _mockUserRepo
            .Setup(m => m.GetUserByEmailAsync("squidlauncher@bankofevil.com"))
            .ReturnsAsync(user);

        var request = new GetUserByEmailQuery("squidlauncher@bankofevil.com");

        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ApplicationUser>());
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo("13"));
            Assert.That(result.Email, Is.EqualTo("squidlauncher@bankofevil.com"));
            Assert.That(result.EmployeeId, Is.EqualTo("abc"));
            Assert.That(result.IsActive, Is.EqualTo(true));
            Assert.That(result.IsScimProvisioned, Is.EqualTo(false));
        });
    }

    [Test]
    public async Task HandleGetUserByEmail_NotFound_Test()
    {
        _ = _mockUserRepo
            .Setup(m => m.GetUserByEmailAsync("Vector"))
            .ReturnsAsync((ApplicationUser)null!);
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
                    { AuthorizationConstants.Actions.GET, true },
                }
            );
        var request = new GetUserByEmailQuery("Vector");

        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetUserByEmail_AuthorizationFailsThrowsTest()
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
                    { AuthorizationConstants.Actions.GET, false },
                }
            );

        var request = new GetUserByEmailQuery("Vector");

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

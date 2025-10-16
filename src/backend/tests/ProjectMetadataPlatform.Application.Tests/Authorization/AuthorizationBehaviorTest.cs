using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Authorization;
using ProjectMetadataPlatform.Application.Authorization.Models;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Authorization;

[TestFixture]
public class AuthorizationBehaviorTest
{
    private Mock<IHttpContextAccessor> _httpContextAccessor;

    private Mock<IEnforcerWrapper> _enforcer;

    [SetUp]
    public void Setup()
    {
        _enforcer = new Mock<IEnforcerWrapper>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
    }

    [Test]
    public async Task HandleAnyCommandSuccessful()
    {
        var next = new Mock<RequestHandlerDelegate<It.IsAnyType>>();
        _enforcer
            .Setup(m =>
                m.EnforceAsync(
                    It.IsAny<AuthorizationSubject>(),
                    It.IsAny<CreateProjectCommand>(),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(true);

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Email, "max.musterman@test.de"),
                new Claim("JobTitle", "Head of BU, Manager"),
                new Claim("Department", "[BU Media, IT-Development, Team#24]"),
                new Claim("TeamSupport", "[Team#02, Team#01]"),
                new Claim("Company", "Appsfactory Gmbh"),
            ],
            "TestAuthorization"
        );
        var contextUser = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = contextUser };
        _httpContextAccessor.Setup(m => m.HttpContext).Returns(httpContext);

        var command = new CreateProjectCommand(
            ProjectName: "TestName",
            ClientName: "TestClientName",
            OfferId: null,
            Company: "TestCompany",
            CompanyState: CompanyState.EXTERNAL,
            TeamId: null,
            IsmsLevel: SecurityLevel.NORMAL,
            Plugins: [],
            Notes: ""
        );

        var authorizationBehavior = new AuthorizationBehavior<CreateProjectCommand, It.IsAnyType>(
            _enforcer.Object,
            _httpContextAccessor.Object
        );

        _ = await authorizationBehavior.Handle(command, next.Object, It.IsAny<CancellationToken>());

        _enforcer.Verify(
            e =>
                e.EnforceAsync(
                    It.Is<AuthorizationSubject>(subject =>
                        subject.Email == "max.musterman@test.de"
                        && subject.JobTitles.Count() == 2
                        && subject.JobTitles.ToList()[0] == "Head of BU"
                        && subject.JobTitles.ToList()[1] == "Manager"
                        && subject.BusinessUnits.Count() == 1
                        && subject.BusinessUnits.ToList()[0] == "BU Media"
                        && subject.Departments.Count() == 1
                        && subject.Departments.ToList()[0] == "IT-Development"
                        && subject.Teams.Count() == 1
                        && subject.Teams.ToList()[0] == "Team#24"
                        && subject.TeamSupport.Count() == 2
                        && subject.TeamSupport.ToList()[0] == "Team#02"
                        && subject.TeamSupport.ToList()[1] == "Team#01"
                        && subject.Company == "Appsfactory Gmbh"
                    ),
                    It.Is<CreateProjectCommand>(request => request == command),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.Is<string>(action => action == nameof(CreateProjectCommand))
                ),
            Times.Once()
        );

        next.Verify(next => next(), Times.Once);
    }

    [Test]
    public async Task HandleAnyCommandThrows()
    {
        var next = new Mock<RequestHandlerDelegate<It.IsAnyType>>();
        _enforcer
            .Setup(m =>
                m.EnforceAsync(
                    It.IsAny<AuthorizationSubject>(),
                    It.IsAny<CreateProjectCommand>(),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(false);

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Email, "max.musterman@test.de"),
                new Claim("JobTitle", "Head of BU, Manager"),
                new Claim("Department", "[BU Media, IT-Development, Team#24]"),
                new Claim("TeamSupport", "[Team#02, Team#01]"),
                new Claim("Company", "Appsfactory Gmbh"),
            ],
            "TestAuthorization"
        );
        var contextUser = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = contextUser };
        _httpContextAccessor.Setup(m => m.HttpContext).Returns(httpContext);

        var command = new CreateProjectCommand(
            ProjectName: "TestName",
            ClientName: "TestClientName",
            OfferId: null,
            Company: "TestCompany",
            CompanyState: CompanyState.EXTERNAL,
            TeamId: null,
            IsmsLevel: SecurityLevel.NORMAL,
            Plugins: [],
            Notes: ""
        );

        var authorizationBehavior = new AuthorizationBehavior<CreateProjectCommand, It.IsAnyType>(
            _enforcer.Object,
            _httpContextAccessor.Object
        );
        var ex = Assert.ThrowsAsync<UnauthorizedException>(async () =>
        {
            await authorizationBehavior.Handle(command, next.Object, It.IsAny<CancellationToken>());
        });

        Assert.That(ex.Message, Is.EqualTo("User is unauthorized to access this ressource."));

        _enforcer.Verify(
            e =>
                e.EnforceAsync(
                    It.Is<AuthorizationSubject>(subject =>
                        subject.Email == "max.musterman@test.de"
                        && subject.JobTitles.Count() == 2
                        && subject.JobTitles.ToList()[0] == "Head of BU"
                        && subject.JobTitles.ToList()[1] == "Manager"
                        && subject.BusinessUnits.Count() == 1
                        && subject.BusinessUnits.ToList()[0] == "BU Media"
                        && subject.Departments.Count() == 1
                        && subject.Departments.ToList()[0] == "IT-Development"
                        && subject.Teams.Count() == 1
                        && subject.Teams.ToList()[0] == "Team#24"
                        && subject.TeamSupport.Count() == 2
                        && subject.TeamSupport.ToList()[0] == "Team#02"
                        && subject.TeamSupport.ToList()[1] == "Team#01"
                        && subject.Company == "Appsfactory Gmbh"
                    ),
                    It.Is<CreateProjectCommand>(request => request == command),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.Is<string>(action => action == nameof(CreateProjectCommand))
                ),
            Times.Once()
        );

        next.Verify(next => next(), Times.Never);
    }

    [Test]
    public async Task HandleAnyQuerySuccessfulNonEnumerableResult()
    {
        _enforcer
            .Setup(m =>
                m.EnforceAsync(
                    It.IsAny<AuthorizationSubject>(),
                    It.IsAny<IdentityUser>(),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(true);

        var next = new Mock<RequestHandlerDelegate<IdentityUser>>();
        next.Setup(m => m()).ReturnsAsync(new IdentityUser { Email = "max.musterman@test.de" });

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Email, "max.musterman@test.de"),
                new Claim("JobTitle", "Head of BU, Manager"),
                new Claim("Department", "[BU Media, IT-Development, Team#24]"),
                new Claim("TeamSupport", "[Team#02, Team#01]"),
                new Claim("Company", "Appsfactory Gmbh"),
            ],
            "TestAuthorization"
        );
        var contextUser = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = contextUser };
        _httpContextAccessor.Setup(m => m.HttpContext).Returns(httpContext);

        var query = new GetUserByEmailQuery("max.musterman@test.de");

        var authorizationBehavior = new AuthorizationBehavior<GetUserByEmailQuery, IdentityUser>(
            _enforcer.Object,
            _httpContextAccessor.Object
        );

        var result = await authorizationBehavior.Handle(
            query,
            next.Object,
            It.IsAny<CancellationToken>()
        );

        _enforcer.Verify(
            e =>
                e.EnforceAsync(
                    It.Is<AuthorizationSubject>(subject =>
                        subject.Email == "max.musterman@test.de"
                        && subject.JobTitles.Count() == 2
                        && subject.JobTitles.ToList()[0] == "Head of BU"
                        && subject.JobTitles.ToList()[1] == "Manager"
                        && subject.BusinessUnits.Count() == 1
                        && subject.BusinessUnits.ToList()[0] == "BU Media"
                        && subject.Departments.Count() == 1
                        && subject.Departments.ToList()[0] == "IT-Development"
                        && subject.Teams.Count() == 1
                        && subject.Teams.ToList()[0] == "Team#24"
                        && subject.TeamSupport.Count() == 2
                        && subject.TeamSupport.ToList()[0] == "Team#02"
                        && subject.TeamSupport.ToList()[1] == "Team#01"
                        && subject.Company == "Appsfactory Gmbh"
                    ),
                    It.Is<IdentityUser>(request => request == result),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.Is<string>(action => action == nameof(GetUserByEmailQuery))
                ),
            Times.Once()
        );

        next.Verify(next => next(), Times.Once);
    }

    [Test]
    public async Task HandleAnyQueryThrowsNonEnumerableResult()
    {
        _enforcer
            .Setup(m =>
                m.EnforceAsync(
                    It.IsAny<AuthorizationSubject>(),
                    It.IsAny<IdentityUser>(),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(false);

        var next = new Mock<RequestHandlerDelegate<IdentityUser>>();
        next.Setup(m => m()).ReturnsAsync(new IdentityUser { Email = "max.musterman@test.de" });

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Email, "max.musterman@test.de"),
                new Claim("JobTitle", "Head of BU, Manager"),
                new Claim("Department", "[BU Media, IT-Development, Team#24]"),
                new Claim("TeamSupport", "[Team#02, Team#01]"),
                new Claim("Company", "Appsfactory Gmbh"),
            ],
            "TestAuthorization"
        );
        var contextUser = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = contextUser };
        _httpContextAccessor.Setup(m => m.HttpContext).Returns(httpContext);

        var query = new GetUserByEmailQuery("max.musterman@test.de");

        var authorizationBehavior = new AuthorizationBehavior<GetUserByEmailQuery, IdentityUser>(
            _enforcer.Object,
            _httpContextAccessor.Object
        );

        var ex = Assert.ThrowsAsync<UnauthorizedException>(async () =>
        {
            await authorizationBehavior.Handle(query, next.Object, It.IsAny<CancellationToken>());
        });

        Assert.That(ex.Message, Is.EqualTo("User is unauthorized to access this ressource."));

        _enforcer.Verify(
            e =>
                e.EnforceAsync(
                    It.Is<AuthorizationSubject>(subject =>
                        subject.Email == "max.musterman@test.de"
                        && subject.JobTitles.Count() == 2
                        && subject.JobTitles.ToList()[0] == "Head of BU"
                        && subject.JobTitles.ToList()[1] == "Manager"
                        && subject.BusinessUnits.Count() == 1
                        && subject.BusinessUnits.ToList()[0] == "BU Media"
                        && subject.Departments.Count() == 1
                        && subject.Departments.ToList()[0] == "IT-Development"
                        && subject.Teams.Count() == 1
                        && subject.Teams.ToList()[0] == "Team#24"
                        && subject.TeamSupport.Count() == 2
                        && subject.TeamSupport.ToList()[0] == "Team#02"
                        && subject.TeamSupport.ToList()[1] == "Team#01"
                        && subject.Company == "Appsfactory Gmbh"
                    ),
                    It.Is<IdentityUser>(request => request.Email == "max.musterman@test.de"),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.Is<string>(action => action == nameof(GetUserByEmailQuery))
                ),
            Times.Once()
        );

        next.Verify(next => next(), Times.Once);
    }

    [Test]
    public async Task HandleAnyQuerySuccessfulEnumerableResult()
    {
        _enforcer
            .Setup(m =>
                m.EnforceAsync(
                    It.IsAny<AuthorizationSubject>(),
                    It.IsAny<IdentityUser>(),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(true);

        var next = new Mock<RequestHandlerDelegate<IEnumerable<IdentityUser>>>();
        next.Setup(m => m())
            .ReturnsAsync(
                [new IdentityUser { Email = "Test1" }, new IdentityUser { Email = "Test2" }]
            );

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Email, "max.musterman@test.de"),
                new Claim("JobTitle", "Head of BU, Manager"),
                new Claim("Department", "[BU Media, IT-Development, Team#24]"),
                new Claim("TeamSupport", "[Team#02, Team#01]"),
                new Claim("Company", "Appsfactory Gmbh"),
            ],
            "TestAuthorization"
        );
        var contextUser = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = contextUser };
        _httpContextAccessor.Setup(m => m.HttpContext).Returns(httpContext);

        var query = new GetAllUsersQuery();

        var authorizationBehavior = new AuthorizationBehavior<
            GetAllUsersQuery,
            IEnumerable<IdentityUser>
        >(_enforcer.Object, _httpContextAccessor.Object);

        var result = await authorizationBehavior.Handle(
            query,
            next.Object,
            It.IsAny<CancellationToken>()
        );

        _enforcer.Verify(
            e =>
                e.EnforceAsync(
                    It.Is<AuthorizationSubject>(subject =>
                        subject.Email == "max.musterman@test.de"
                        && subject.JobTitles.Count() == 2
                        && subject.JobTitles.ToList()[0] == "Head of BU"
                        && subject.JobTitles.ToList()[1] == "Manager"
                        && subject.BusinessUnits.Count() == 1
                        && subject.BusinessUnits.ToList()[0] == "BU Media"
                        && subject.Departments.Count() == 1
                        && subject.Departments.ToList()[0] == "IT-Development"
                        && subject.Teams.Count() == 1
                        && subject.Teams.ToList()[0] == "Team#24"
                        && subject.TeamSupport.Count() == 2
                        && subject.TeamSupport.ToList()[0] == "Team#02"
                        && subject.TeamSupport.ToList()[1] == "Team#01"
                        && subject.Company == "Appsfactory Gmbh"
                    ),
                    It.Is<IdentityUser>(request => result.Contains(request)),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.Is<string>(action => action == nameof(GetAllUsersQuery))
                ),
            Times.Exactly(2)
        );

        next.Verify(next => next(), Times.Once);
    }

    [Test]
    public async Task HandleAnyQueryThrowsEnumerableResult()
    {
        _enforcer
            .Setup(m =>
                m.EnforceAsync(
                    It.IsAny<AuthorizationSubject>(),
                    It.IsAny<IdentityUser>(),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(false);

        var next = new Mock<RequestHandlerDelegate<IEnumerable<IdentityUser>>>();
        next.Setup(m => m())
            .ReturnsAsync(
                [new IdentityUser { Email = "Test1" }, new IdentityUser { Email = "Test2" }]
            );

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Email, "max.musterman@test.de"),
                new Claim("JobTitle", "Head of BU, Manager"),
                new Claim("Department", "[BU Media, IT-Development, Team#24]"),
                new Claim("TeamSupport", "[Team#02, Team#01]"),
                new Claim("Company", "Appsfactory Gmbh"),
            ],
            "TestAuthorization"
        );
        var contextUser = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = contextUser };
        _httpContextAccessor.Setup(m => m.HttpContext).Returns(httpContext);

        var query = new GetAllUsersQuery();

        var authorizationBehavior = new AuthorizationBehavior<
            GetAllUsersQuery,
            IEnumerable<IdentityUser>
        >(_enforcer.Object, _httpContextAccessor.Object);

        var ex = Assert.ThrowsAsync<UnauthorizedException>(async () =>
        {
            await authorizationBehavior.Handle(query, next.Object, It.IsAny<CancellationToken>());
        });

        Assert.That(ex.Message, Is.EqualTo("User is unauthorized to access this ressource."));

        _enforcer.Verify(
            e =>
                e.EnforceAsync(
                    It.Is<AuthorizationSubject>(subject =>
                        subject.Email == "max.musterman@test.de"
                        && subject.JobTitles.Count() == 2
                        && subject.JobTitles.ToList()[0] == "Head of BU"
                        && subject.JobTitles.ToList()[1] == "Manager"
                        && subject.BusinessUnits.Count() == 1
                        && subject.BusinessUnits.ToList()[0] == "BU Media"
                        && subject.Departments.Count() == 1
                        && subject.Departments.ToList()[0] == "IT-Development"
                        && subject.Teams.Count() == 1
                        && subject.Teams.ToList()[0] == "Team#24"
                        && subject.TeamSupport.Count() == 2
                        && subject.TeamSupport.ToList()[0] == "Team#02"
                        && subject.TeamSupport.ToList()[1] == "Team#01"
                        && subject.Company == "Appsfactory Gmbh"
                    ),
                    It.Is<IdentityUser>(request => request.Email == "Test1"),
                    It.IsAny<AuthorizationEnvironment>(),
                    It.Is<string>(action => action == nameof(GetAllUsersQuery))
                ),
            Times.Once()
        );

        next.Verify(next => next(), Times.Once);
    }
}

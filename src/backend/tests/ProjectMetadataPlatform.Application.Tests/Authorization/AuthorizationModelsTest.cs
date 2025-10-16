using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Time.Testing;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Authorization.Models;

namespace ProjectMetadataPlatform.Application.Tests.Authorization;

[TestFixture]
public class AuthorizationModelsTest
{
    [Test]
    public async Task ConvertClaimsToAuthorizationSubjectTest()
    {
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

        var result = AuthorizationSubject.ConvertClaimsToAuthorizationSubject(contextUser);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf(typeof(AuthorizationSubject)));
            Assert.That(result.Email, Is.EqualTo("max.musterman@test.de"));

            Assert.That(result.JobTitles.Count, Is.EqualTo(2));
            Assert.That(result.JobTitles.ToList()[0], Is.EqualTo("Head of BU"));
            Assert.That(result.JobTitles.ToList()[1], Is.EqualTo("Manager"));

            Assert.That(result.BusinessUnits.Count, Is.EqualTo(1));
            Assert.That(result.BusinessUnits.ToList()[0], Is.EqualTo("BU Media"));

            Assert.That(result.Departments.Count, Is.EqualTo(1));
            Assert.That(result.Departments.ToList()[0], Is.EqualTo("IT-Development"));

            Assert.That(result.Teams.Count, Is.EqualTo(1));
            Assert.That(result.Teams.ToList()[0], Is.EqualTo("Team#24"));

            Assert.That(result.TeamSupport.Count, Is.EqualTo(2));
            Assert.That(result.TeamSupport.ToList()[0], Is.EqualTo("Team#02"));
            Assert.That(result.TeamSupport.ToList()[1], Is.EqualTo("Team#01"));

            Assert.That(result.Company, Is.EqualTo("Appsfactory Gmbh"));
        });
    }

    [Test]
    public async Task CreateAuthorizationEnvironmentTest()
    {
        var fakeTime = new FakeTimeProvider();

        var result = AuthorizationEnvironment.CreateAuthorizationEnvironment(fakeTime);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf(typeof(AuthorizationEnvironment)));
            Assert.That(result.DateTime, Is.EqualTo(fakeTime.GetUtcNow().ToLocalTime()));
        });
    }
}

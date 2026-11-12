using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Helper;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using IAuthorizationService = ProjectMetadataPlatform.Application.Interfaces.IAuthorizationService;

namespace ProjectMetadataPlatform.Application.Tests.Helper;

internal class TestResource
{
    public int Id { get; set; }
};

[TestFixture]
public class PaginationHelperTest
{
    private Mock<IAuthorizationService> _authorizationServiceMock;

    private IPaginationHelper _paginationHelper;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelper = new PaginationHelper(_authorizationServiceMock.Object);
    }

    [Test]
    public async Task PaginateWithAuthAsync_QueriedResourcesWithoutLimit_ReturnAllQueriedResourcesWithoutLastResource()
    {
        var resourceList = new List<TestResource>
        {
            new() { Id = 1 },
            new() { Id = 2 },
            new() { Id = 3 },
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<TestResource>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<TestResource>? query, Dictionary<string, string>? dict) => query
            );
        var (resources, lastResource) = await _paginationHelper.PaginateWithAuthAsync<TestResource>(
            resourceList.BuildMock(),
            null
        );
        _authorizationServiceMock.Verify(
            a =>
                a.CheckAccess(
                    It.IsAny<TestResource>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                ),
            Times.Never
        );
        Assert.That(resources, Is.EquivalentTo(resourceList));
        Assert.Null(lastResource);
    }

    [Test]
    public async Task PaginateWithAuthAsync_QueriedResourcesWithBigLimit_ReturnAllQueriedResourcesWithoutLastResource()
    {
        var resourceList = new List<TestResource>
        {
            new() { Id = 1 },
            new() { Id = 2 },
            new() { Id = 3 },
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<TestResource>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<TestResource>? query, Dictionary<string, string>? dict) => query
            );
        var (resources, lastResource) = await _paginationHelper.PaginateWithAuthAsync<TestResource>(
            resourceList.BuildMock(),
            20
        );
        _authorizationServiceMock.Verify(
            a =>
                a.CheckAccess(
                    It.IsAny<TestResource>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                ),
            Times.Never
        );
        Assert.That(resources, Is.EquivalentTo(resourceList));
        Assert.Null(lastResource);
    }

    [Test]
    public async Task PaginateWithAuthAsync_QueriedResourcesLimit_ReturnAllQueriedResourcesWithLastResource()
    {
        var resourceList = new List<TestResource>
        {
            new() { Id = 1 },
            new() { Id = 2 },
            new() { Id = 3 },
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<TestResource>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<TestResource>? query, Dictionary<string, string>? dict) => query
            );
        var (resources, lastResource) = await _paginationHelper.PaginateWithAuthAsync<TestResource>(
            resourceList.BuildMock(),
            2
        );
        _authorizationServiceMock.Verify(
            a =>
                a.CheckAccess(
                    It.IsAny<TestResource>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                ),
            Times.Never
        );
        using (Assert.EnterMultipleScope())
        {
            Assert.That(resources, Is.EquivalentTo(resourceList[0..2]));
            Assert.That(lastResource, Is.EqualTo(resourceList[1]));
        }
    }

    [Test]
    public async Task PaginateWithAuthAsync_FilteredResourcesWithoutLimit_ReturnAllQueriedResourcesWithoutLastResource()
    {
        var resourceList = new List<TestResource>
        {
            new() { Id = 1 },
            new() { Id = 2 },
            new() { Id = 3 },
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<TestResource>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<TestResource>?)null);

        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<TestResource>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);

        var (resources, lastResource) = await _paginationHelper.PaginateWithAuthAsync<TestResource>(
            resourceList.BuildMock(),
            null
        );

        Assert.That(resources, Is.EquivalentTo(resourceList));
        Assert.Null(lastResource);
    }

    [Test]
    public async Task PaginateWithAuthAsync_FilteredResourcesWithBigLimit_ReturnAllQueriedResourcesWithoutLastResource()
    {
        var resourceList = new List<TestResource>
        {
            new() { Id = 1 },
            new() { Id = 2 },
            new() { Id = 3 },
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<TestResource>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<TestResource>?)null);

        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<TestResource>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        var (resources, lastResource) = await _paginationHelper.PaginateWithAuthAsync<TestResource>(
            resourceList.BuildMock(),
            20
        );
        Assert.That(resources, Is.EquivalentTo(resourceList));
        Assert.Null(lastResource);
    }

    [Test]
    public async Task PaginateWithAuthAsync_FilteredResourcesLimit_ReturnAllQueriedResourcesWithLastResource()
    {
        var resourceList = new List<TestResource>
        {
            new() { Id = 1 },
            new() { Id = 2 },
            new() { Id = 3 },
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<TestResource>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<TestResource>?)null);

        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<TestResource>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);

        var (resources, lastResource) = await _paginationHelper.PaginateWithAuthAsync<TestResource>(
            resourceList.BuildMock(),
            2
        );
        using (Assert.EnterMultipleScope())
        {
            Assert.That(resources, Is.EquivalentTo(resourceList[0..2]));
            Assert.That(lastResource, Is.EqualTo(resourceList[1]));
        }
    }
}

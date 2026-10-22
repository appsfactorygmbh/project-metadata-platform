using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class GetAllUsersQueryHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelperMock = new Mock<IPaginationHelper>();
        _mockUserRepo = new Mock<IUsersRepository>();
        _handler = new GetAllUsersQueryHandler(
            _mockUserRepo.Object,
            authorizationService: _authorizationServiceMock.Object,
            _paginationHelperMock.Object
        );
    }

    private GetAllUsersQueryHandler _handler;
    private Mock<IUsersRepository> _mockUserRepo;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IPaginationHelper> _paginationHelperMock;

    [Test]
    public async Task HandleGetAllUsersRequest_EmptyResponse_Test()
    {
        _ = _paginationHelperMock
            .Setup(a =>
                a.PaginateWithAuthAsync(It.IsAny<IQueryable<ApplicationUser>>(), It.IsAny<int?>())
            )
            .ReturnsAsync((IQueryable<ApplicationUser> query, int? limit) => (query, null));
        _ = _mockUserRepo
            .Setup(m => m.GetUsersAsync("", null))
            .ReturnsAsync(new List<ApplicationUser> { }.BuildMock());
        var request = new GetAllUsersQuery("", null, null);
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        ApplicationUser[] resultArray = result.Item1 as ApplicationUser[] ?? result.Item1.ToArray();
        Assert.That(resultArray, Is.Not.Null);
        Assert.That(resultArray, Is.InstanceOf<IEnumerable<ApplicationUser>>());

        Assert.That(resultArray, Has.Length.EqualTo(0));
    }

    [Test]
    public async Task HandleGetAllUsersRequest_Test()
    {
        var usersResponseContent = new List<ApplicationUser>
        {
            new()
            {
                EmployeeId = "abc",
                Id = "1",
                Email = "Hinz",
                IsActive = true,
                IsScimProvisioned = false,
            },
        };
        _ = _paginationHelperMock
            .Setup(a =>
                a.PaginateWithAuthAsync(It.IsAny<IQueryable<ApplicationUser>>(), It.IsAny<int?>())
            )
            .ReturnsAsync((IQueryable<ApplicationUser> query, int? limit) => (query, null));
        _ = _mockUserRepo
            .Setup(m => m.GetUsersAsync("qweqweqwe", null))
            .ReturnsAsync(usersResponseContent.BuildMock());
        var request = new GetAllUsersQuery("qweqweqwe", null, null);
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).Item1.ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<ApplicationUser>>());

        var resultArray = result.ToArray();
        Assert.That(resultArray, Has.Length.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(resultArray[0].Id, Is.EqualTo("1"));
            Assert.That(resultArray[0].Email, Is.EqualTo("Hinz"));
            Assert.That(resultArray[0].EmployeeId, Is.EqualTo("abc"));
            Assert.That(resultArray[0].IsActive, Is.True);
            Assert.That(resultArray[0].IsScimProvisioned, Is.False);
        });
    }
}

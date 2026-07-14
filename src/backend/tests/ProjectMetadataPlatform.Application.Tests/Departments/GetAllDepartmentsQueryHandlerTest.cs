using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Departments;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Tests.Departments;

[TestFixture]
public class GetAllDepartmentsQueryHandlerTest
{
    private GetAllDepartmentsQueryHandler _handler;
    private Mock<IDepartmentRepository> _mockDepartmentRepository;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _handler = new GetAllDepartmentsQueryHandler(
            departmentRepository: _mockDepartmentRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetAllDepartments_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnDepartment = new Department() { Id = 1, DepartmentName = "Test_1" };

        _ = _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentsAsync())
            .ReturnsAsync(new List<Department> { returnDepartment }.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Department>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<Department> query, Dictionary<string, string>? dict) => query
            );
        // Act
        var result = await _handler.Handle(
            new GetAllDepartmentsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1.Count, Is.EqualTo(1));
        Assert.That(result.Item1.First(), Is.EqualTo(returnDepartment));
        _mockDepartmentRepository.Verify(m => m.GetDepartmentsAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllDepartments_ReturnsInOrder()
    {
        // Arrange
        List<Department> returnDepartment =
        [
            new() { Id = 1, DepartmentName = "Test_1" },
            new() { Id = 3, DepartmentName = "test_3" },
            new() { Id = 2, DepartmentName = "TesT_2" },
            new() { Id = 4, DepartmentName = "Foo_2" },
        ];

        _ = _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentsAsync())
            .ReturnsAsync(returnDepartment.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Department>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<Department> query, Dictionary<string, string>? dict) => query
            );
        // Act
        var result = await _handler.Handle(
            new GetAllDepartmentsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        var resultList = result.Item1.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.Item1.Count, Is.EqualTo(4));
            Assert.That(resultList[0], Is.EqualTo(returnDepartment[3]));
            Assert.That(resultList[1], Is.EqualTo(returnDepartment[0]));
            Assert.That(resultList[2], Is.EqualTo(returnDepartment[2]));
            Assert.That(resultList[3], Is.EqualTo(returnDepartment[1]));
        });
    }

    [Test]
    public async Task GetAllDepartments_ReturnsFilteredInOrder()
    {
        // Arrange
        List<Department> returnDepartment =
        [
            new() { Id = 1, DepartmentName = "Test_1" },
            new() { Id = 3, DepartmentName = "test_3" },
            new() { Id = 2, DepartmentName = "TesT_2" },
            new() { Id = 4, DepartmentName = "Foo_2" },
        ];

        _ = _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentsAsync())
            .ReturnsAsync(returnDepartment.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Department>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<Department>?)null);
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Department>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        // Act
        var result = await _handler.Handle(
            new GetAllDepartmentsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        var resultList = result.Item1.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.Item1.Count, Is.EqualTo(4));
            Assert.That(resultList[0], Is.EqualTo(returnDepartment[3]));
            Assert.That(resultList[1], Is.EqualTo(returnDepartment[0]));
            Assert.That(resultList[2], Is.EqualTo(returnDepartment[2]));
            Assert.That(resultList[3], Is.EqualTo(returnDepartment[1]));
        });
    }
}

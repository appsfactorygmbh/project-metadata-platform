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
    private Mock<IPaginationHelper> _paginationHelperMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelperMock = new Mock<IPaginationHelper>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _handler = new GetAllDepartmentsQueryHandler(
            departmentRepository: _mockDepartmentRepository.Object,
            authorizationService: _authorizationServiceMock.Object,
            _paginationHelperMock.Object
        );
    }

    [Test]
    public async Task GetAllDepartments_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnDepartment = new Department() { Id = 1, DepartmentName = "Test_1" };

        _ = _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentsAsync(null))
            .ReturnsAsync(new List<Department> { returnDepartment }.BuildMock());
        _ = _paginationHelperMock
            .Setup(a =>
                a.PaginateWithAuthAsync(It.IsAny<IQueryable<Department>>(), It.IsAny<int?>())
            )
            .ReturnsAsync((IQueryable<Department> query, int? limit) => (query, null));
        // Act
        var result = await _handler.Handle(
            new GetAllDepartmentsQuery(null, null),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1.Count, Is.EqualTo(1));
        Assert.That(result.Item1.First(), Is.EqualTo(returnDepartment));
        _mockDepartmentRepository.Verify(m => m.GetDepartmentsAsync(null), Times.Once);
    }
}

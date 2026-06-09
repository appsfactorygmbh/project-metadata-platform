using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Departments;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Application.Tests.Departments;

[TestFixture]
public class GetAllDepartmentsQueryHandlerTest
{
    private GetAllDepartmentsQueryHandler _handler;
    private Mock<IDepartmentRepository> _mockDepartmentRepository;

    [SetUp]
    public void Setup()
    {
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _handler = new GetAllDepartmentsQueryHandler(
            departmentRepository: _mockDepartmentRepository.Object
        );
    }

    [Test]
    public async Task GetAllDepartments_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnDepartment = new Department() { Id = 1, DepartmentName = "Test_1" };

        _ = _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentsAsync())
            .ReturnsAsync([returnDepartment]);

        // Act
        var result = await _handler.Handle(
            new GetAllDepartmentsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo(returnDepartment));
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
            .ReturnsAsync(returnDepartment);

        // Act
        var result = await _handler.Handle(
            new GetAllDepartmentsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        var resultList = result.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(resultList[0], Is.EqualTo(returnDepartment[3]));
            Assert.That(resultList[1], Is.EqualTo(returnDepartment[0]));
            Assert.That(resultList[2], Is.EqualTo(returnDepartment[2]));
            Assert.That(resultList[3], Is.EqualTo(returnDepartment[1]));
        });
    }
}

using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Departments;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;

namespace ProjectMetadataPlatform.Application.Tests.Departments;

[TestFixture]
public class GetDepartmentQueryHandlerTest
{
    private GetDepartmentQueryHandler _handler;
    private Mock<IDepartmentRepository> _mockDepartmentRepository;

    [SetUp]
    public void Setup()
    {
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _handler = new GetDepartmentQueryHandler(
            departmentRepository: _mockDepartmentRepository.Object
        );
    }

    [Test]
    public async Task GetDepartment_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnDepartment = new Department() { Id = 1, DepartmentName = "Test_1" };

        _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentAsync(It.IsAny<int>()))
            .ReturnsAsync(returnDepartment);

        // Act
        var result = await _handler.Handle(
            new GetDepartmentQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnDepartment));
        _mockDepartmentRepository.Verify(
            m => m.GetDepartmentAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
    }

    [Test]
    public void GetDepartment_ThrowDepartmentNotFoundException_IfDepartmentNotFound()
    {
        // Arrange
        _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentAsync(It.IsAny<int>()))
            .ThrowsAsync(new DepartmentNotFoundException(1));

        // Act + Assert
        var ex = Assert.ThrowsAsync<DepartmentNotFoundException>(async () =>
            await _handler.Handle(new GetDepartmentQuery(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }
}

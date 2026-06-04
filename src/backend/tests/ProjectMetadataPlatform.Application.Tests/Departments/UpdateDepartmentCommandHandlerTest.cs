using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Departments;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Tests.Departments;

[TestFixture]
public class UpdateDepartmentCommandHandlerTest
{
    private UpdateDepartmentCommandHandler _handler;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    private Mock<IDepartmentRepository> _mockDepartmentRepository;

    [SetUp]
    public void Setup()
    {
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new UpdateDepartmentCommandHandler(
            departmentRepository: _mockDepartmentRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task UpdateDepartment_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnDepartment = new Department() { Id = 1, DepartmentName = "Test_1" };

        _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentAsync(It.IsAny<int>()))
            .ReturnsAsync(returnDepartment);

        _mockDepartmentRepository
            .Setup(repo => repo.UpdateDepartmentAsync(It.IsAny<Department>()))
            .ReturnsAsync((Department department) => department);

        _mockDepartmentRepository
            .Setup(repo => repo.CheckIfDepartmentNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new UpdateDepartmentCommand(Id: 1, DepartmentName: "Test_2"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnDepartment));
        _mockDepartmentRepository.Verify(
            m => m.GetDepartmentAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
        _mockDepartmentRepository.Verify(
            m =>
                m.UpdateDepartmentAsync(
                    It.Is<Department>(department =>
                        department.Id == 1 && department.DepartmentName == "Test_2"
                    )
                ),
            Times.Once
        );
        _mockLogRepo.Verify(
            m =>
                m.AddDepartmentLogForCurrentActor(
                    It.IsAny<Department>(),
                    Action.UPDATED_DEPARTMENT,
                    It.Is<List<LogChange>>(changes =>
                        changes[0].Property == "DepartmentName"
                        && changes[0].NewValue == "Test_2"
                        && changes[0].OldValue == "Test_1"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task UpdateDepartment_NoLogCreatedIfValuesAreEqual()
    {
        // Arrange
        var returnDepartment = new Department() { Id = 1, DepartmentName = "Test_1" };

        _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentAsync(It.IsAny<int>()))
            .ReturnsAsync(returnDepartment);

        _mockDepartmentRepository
            .Setup(repo => repo.UpdateDepartmentAsync(It.IsAny<Department>()))
            .ReturnsAsync((Department department) => department);

        _mockDepartmentRepository
            .Setup(repo => repo.CheckIfDepartmentNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new UpdateDepartmentCommand(Id: 1, DepartmentName: "Test_1"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddDepartmentLogForCurrentActor(
                    It.IsAny<Department>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }

    [Test]
    public void UpdateDepartment_ThrowsDepartmentNameAlreadyExistsException_IfNewDepartmentNameAlreadyExists()
    {
        // Arrange
        var returnDepartment = new Department() { Id = 1, DepartmentName = "Test_1" };

        _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentAsync(It.IsAny<int>()))
            .ReturnsAsync(returnDepartment);

        _mockDepartmentRepository
            .Setup(repo => repo.UpdateDepartmentAsync(It.IsAny<Department>()))
            .ReturnsAsync((Department department) => department);

        _mockDepartmentRepository
            .Setup(repo => repo.CheckIfDepartmentNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act + Assert
        var ex = Assert.ThrowsAsync<DepartmentNameAlreadyExistsException>(async () =>
            await _handler.Handle(
                new UpdateDepartmentCommand(Id: 1, DepartmentName: "Test_2"),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test_2"));
    }
}

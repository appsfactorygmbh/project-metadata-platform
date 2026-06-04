using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Departments;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Departments;

[TestFixture]
public class CreateDepartmentCommandHandlerTest
{
    private CreateDepartmentCommandHandler _handler;
    private Mock<IDepartmentRepository> _mockDepartmentRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();

        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateDepartmentCommandHandler(
            departmentRepository: _mockDepartmentRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task CreateDepartment_NameDoesNotAlreadyExists_WorksFine()
    {
        // Arrange
        _mockDepartmentRepository
            .Setup(repo => repo.CheckIfDepartmentNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockDepartmentRepository
            .Setup(repo => repo.AddDepartmentAsync(It.IsAny<Department>()))
            .Callback(
                (Department departmentBeingAdded) =>
                {
                    departmentBeingAdded.Id = 1;
                }
            )
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(
            new CreateDepartmentCommand(DepartmentName: "Test Name"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.EqualTo(1));
        _mockLogRepo.Verify(
            m =>
                m.AddDepartmentLogForCurrentActor(
                    It.IsAny<Department>(),
                    Action.ADDED_DEPARTMENT,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockDepartmentRepository.Verify(
            m =>
                m.AddDepartmentAsync(
                    It.Is<Department>(department => department.DepartmentName == "Test Name")
                ),
            Times.Once
        );
    }

    [Test]
    public void CreateDepartment_NameAlreadyExists_ThrowsDepartmentNameAlreadyExistsException()
    {
        // Arrange
        _mockDepartmentRepository
            .Setup(repo => repo.CheckIfDepartmentNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<DepartmentNameAlreadyExistsException>(async () =>
            await _handler.Handle(
                new CreateDepartmentCommand(DepartmentName: "Test Name"),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test Name"));
    }
}

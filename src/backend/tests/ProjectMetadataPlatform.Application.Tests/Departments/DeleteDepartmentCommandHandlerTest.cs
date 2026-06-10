using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Departments;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Departments;

[TestFixture]
public class DeleteDepartmentCommandHandlerTest
{
    private DeleteDepartmentCommandHandler _handler;
    private Mock<IDepartmentRepository> _mockDepartmentRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteDepartmentCommandHandler(
            departmentRepository: _mockDepartmentRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task DeleteDepartment_WorksFine()
    {
        // Arrange
        var returnDepartment = new Department() { Id = 1, DepartmentName = "Test_1" };

        _ = _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentAsync(It.IsAny<int>()))
            .ReturnsAsync(returnDepartment);

        // Act
        await _handler.Handle(new DeleteDepartmentCommand(Id: 1), It.IsAny<CancellationToken>());

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddDepartmentLogForCurrentActor(
                    It.IsAny<Department>(),
                    Action.REMOVED_DEPARTMENT,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockDepartmentRepository.Verify(
            m =>
                m.DeleteDepartmentAsync(
                    It.Is<Department>(department =>
                        department.Id == 1 && department.DepartmentName == "Test_1"
                    )
                ),
            Times.Once
        );
    }
}

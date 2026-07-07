using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Departments;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
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
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteDepartmentCommandHandler(
            departmentRepository: _mockDepartmentRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task DeleteDepartment_WorksFine()
    {
        // Arrange
        var returnDepartment = new Department() { Id = 1, DepartmentName = "Test_1" };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Department>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.DELETE, true },
                }
            );
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

    [Test]
    public async Task DeleteDepartment_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Department>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.DELETE, false },
                }
            );

        var request = new DeleteDepartmentCommand(Id: 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

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
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateDepartmentCommandHandler(
            departmentRepository: _mockDepartmentRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task CreateDepartment_NameDoesNotAlreadyExists_WorksFine()
    {
        // Arrange
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
                    { AuthorizationConstants.Actions.CREATE, true },
                }
            );
        _ = _mockDepartmentRepository
            .Setup(repo => repo.CheckIfDepartmentNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _ = _mockDepartmentRepository
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
                    { AuthorizationConstants.Actions.CREATE, true },
                }
            );
        _ = _mockDepartmentRepository
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

    [Test]
    public async Task CreateDepartment_AuthorizationFailsThrowsTest()
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
                    { AuthorizationConstants.Actions.CREATE, false },
                }
            );

        var request = new CreateDepartmentCommand(DepartmentName: "Test Name");

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

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

namespace ProjectMetadataPlatform.Application.Tests.Departments;

[TestFixture]
public class GetDepartmentQueryHandlerTest
{
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private GetDepartmentQueryHandler _handler;
    private Mock<IDepartmentRepository> _mockDepartmentRepository;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _handler = new GetDepartmentQueryHandler(
            departmentRepository: _mockDepartmentRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetDepartment_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnDepartment = new Department() { Id = 1, DepartmentName = "Test_1" };

        _ = _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentAsync(It.IsAny<int>()))
            .ReturnsAsync(returnDepartment);
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
            new GetDepartmentQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1, Is.Not.Null);
        Assert.That(result.Item1, Is.EqualTo(returnDepartment));
        _mockDepartmentRepository.Verify(
            m => m.GetDepartmentAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
    }

    [Test]
    public void GetDepartment_ThrowDepartmentNotFoundException_IfDepartmentNotFound()
    {
        // Arrange
        _ = _mockDepartmentRepository
            .Setup(repo => repo.GetDepartmentAsync(It.IsAny<int>()))
            .ThrowsAsync(new DepartmentNotFoundException(1));
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Department>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<DepartmentNotFoundException>(async () =>
            await _handler.Handle(new GetDepartmentQuery(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task GetDepartment_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Department>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);

        var request = new GetDepartmentQuery(Id: 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

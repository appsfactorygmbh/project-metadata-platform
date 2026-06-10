using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.BusinessUnits;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Tests.BusinessUnits;

[TestFixture]
public class UpdateBusinessUnitCommandHandlerTest
{
    private UpdateBusinessUnitCommandHandler _handler;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;

    [SetUp]
    public void Setup()
    {
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new UpdateBusinessUnitCommandHandler(
            businessUnitRepository: _mockBusinessUnitRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task UpdateBusinessUnit_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnBusinessUnit = new BusinessUnit() { Id = 1, BusinessUnitName = "Test_1" };

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBusinessUnit);

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.UpdateBusinessUnitAsync(It.IsAny<BusinessUnit>()))
            .ReturnsAsync((BusinessUnit businessUnit) => businessUnit);

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.CheckIfBusinessUnitNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new UpdateBusinessUnitCommand(Id: 1, BusinessUnitName: "Test_2"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnBusinessUnit));
        _mockBusinessUnitRepository.Verify(
            m => m.GetBusinessUnitAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
        _mockBusinessUnitRepository.Verify(
            m =>
                m.UpdateBusinessUnitAsync(
                    It.Is<BusinessUnit>(businessUnit =>
                        businessUnit.Id == 1 && businessUnit.BusinessUnitName == "Test_2"
                    )
                ),
            Times.Once
        );
        _mockLogRepo.Verify(
            m =>
                m.AddBusinessUnitLogForCurrentActor(
                    It.IsAny<BusinessUnit>(),
                    Action.UPDATED_BUSINESS_UNIT,
                    It.Is<List<LogChange>>(changes =>
                        changes[0].Property == "BusinessUnitName"
                        && changes[0].NewValue == "Test_2"
                        && changes[0].OldValue == "Test_1"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task UpdateBusinessUnit_NoLogCreatedIfValuesAreEqual()
    {
        // Arrange
        var returnBusinessUnit = new BusinessUnit() { Id = 1, BusinessUnitName = "Test_1" };

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBusinessUnit);

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.UpdateBusinessUnitAsync(It.IsAny<BusinessUnit>()))
            .ReturnsAsync((BusinessUnit businessUnit) => businessUnit);

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.CheckIfBusinessUnitNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new UpdateBusinessUnitCommand(Id: 1, BusinessUnitName: "Test_1"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddBusinessUnitLogForCurrentActor(
                    It.IsAny<BusinessUnit>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }

    [Test]
    public void UpdateBusinessUnit_ThrowsBusinessUnitNameAlreadyExistsException_IfNewBusinessUnitNameAlreadyExists()
    {
        // Arrange
        var returnBusinessUnit = new BusinessUnit() { Id = 1, BusinessUnitName = "Test_1" };

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBusinessUnit);

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.UpdateBusinessUnitAsync(It.IsAny<BusinessUnit>()))
            .ReturnsAsync((BusinessUnit businessUnit) => businessUnit);

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.CheckIfBusinessUnitNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act + Assert
        var ex = Assert.ThrowsAsync<BusinessUnitNameAlreadyExistsException>(async () =>
            await _handler.Handle(
                new UpdateBusinessUnitCommand(Id: 1, BusinessUnitName: "Test_2"),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test_2"));
    }
}

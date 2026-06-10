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
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.BusinessUnits;

[TestFixture]
public class DeleteBusinessUnitCommandHandlerTest
{
    private DeleteBusinessUnitCommandHandler _handler;
    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteBusinessUnitCommandHandler(
            businessUnitRepository: _mockBusinessUnitRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task DeleteBusinessUnit_NoLinkedTeams_WorksFine()
    {
        // Arrange
        var returnBusinessUnit = new BusinessUnit()
        {
            Id = 1,
            BusinessUnitName = "Test_1",
            Teams = [],
        };

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitWithTeamsAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBusinessUnit);

        // Act
        await _handler.Handle(new DeleteBusinessUnitCommand(Id: 1), It.IsAny<CancellationToken>());

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddBusinessUnitLogForCurrentActor(
                    It.IsAny<BusinessUnit>(),
                    Action.REMOVED_BUSINESS_UNIT,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockBusinessUnitRepository.Verify(
            m =>
                m.DeleteBusinessUnitAsync(
                    It.Is<BusinessUnit>(businessUnit =>
                        businessUnit.Id == 1 && businessUnit.BusinessUnitName == "Test_1"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public void DeleteBusinessUnit_StillLinkedTeams_ThrowsBusinessUnitStillLinkedToTeamsException()
    {
        // Arrange
        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.CheckIfBusinessUnitNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var returnBusinessUnit = new BusinessUnit()
        {
            Id = 1,
            BusinessUnitName = "Test_1",
            Teams =
            [
                new()
                {
                    Id = 111,
                    TeamName = "Team",
                    BusinessUnitId = 1,
                },
            ],
        };

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitWithTeamsAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBusinessUnit);

        // Act + Assert
        var ex = Assert.ThrowsAsync<BusinessUnitStillLinkedToTeamsException>(async () =>
            await _handler.Handle(
                new DeleteBusinessUnitCommand(Id: 1),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("111"));
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.OfficeLocations;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.OfficeLocations;

[TestFixture]
public class DeleteOfficeLocationCommandHandlerTest
{
    private DeleteOfficeLocationCommandHandler _handler;
    private Mock<IOfficeLocationRepository> _mockOfficeLocationRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockOfficeLocationRepository = new Mock<IOfficeLocationRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteOfficeLocationCommandHandler(
            officeLocationRepository: _mockOfficeLocationRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task DeleteOfficeLocation_WorksFine()
    {
        // Arrange
        var returnOfficeLocation = new OfficeLocation() { Id = 1, OfficeLocationName = "Test_1" };

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationAsync(It.IsAny<int>()))
            .ReturnsAsync(returnOfficeLocation);

        // Act
        await _handler.Handle(
            new DeleteOfficeLocationCommand(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddOfficeLocationLogForCurrentActor(
                    It.IsAny<OfficeLocation>(),
                    Action.REMOVED_OFFICE_LOCATION,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockOfficeLocationRepository.Verify(
            m =>
                m.DeleteOfficeLocationAsync(
                    It.Is<OfficeLocation>(officeLocation =>
                        officeLocation.Id == 1 && officeLocation.OfficeLocationName == "Test_1"
                    )
                ),
            Times.Once
        );
    }
}

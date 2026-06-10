using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.OfficeLocations;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.OfficeLocations;

[TestFixture]
public class CreateOfficeLocationCommandHandlerTest
{
    private CreateOfficeLocationCommandHandler _handler;
    private Mock<IOfficeLocationRepository> _mockOfficeLocationRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockOfficeLocationRepository = new Mock<IOfficeLocationRepository>();

        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateOfficeLocationCommandHandler(
            officeLocationRepository: _mockOfficeLocationRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task CreateOfficeLocation_NameDoesNotAlreadyExists_WorksFine()
    {
        // Arrange
        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.CheckIfOfficeLocationNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.AddOfficeLocationAsync(It.IsAny<OfficeLocation>()))
            .Callback(
                (OfficeLocation officeLocationBeingAdded) =>
                {
                    officeLocationBeingAdded.Id = 1;
                }
            )
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(
            new CreateOfficeLocationCommand(OfficeLocationName: "Test Name"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.EqualTo(1));
        _mockLogRepo.Verify(
            m =>
                m.AddOfficeLocationLogForCurrentActor(
                    It.IsAny<OfficeLocation>(),
                    Action.ADDED_OFFICE_LOCATION,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockOfficeLocationRepository.Verify(
            m =>
                m.AddOfficeLocationAsync(
                    It.Is<OfficeLocation>(officeLocation =>
                        officeLocation.OfficeLocationName == "Test Name"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public void CreateOfficeLocation_NameAlreadyExists_ThrowsOfficeLocationNameAlreadyExistsException()
    {
        // Arrange
        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.CheckIfOfficeLocationNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<OfficeLocationNameAlreadyExistsException>(async () =>
            await _handler.Handle(
                new CreateOfficeLocationCommand(OfficeLocationName: "Test Name"),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test Name"));
    }
}

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

namespace ProjectMetadataPlatform.Application.Tests.OfficeLocations;

[TestFixture]
public class UpdateOfficeLocationCommandHandlerTest
{
    private UpdateOfficeLocationCommandHandler _handler;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    private Mock<IOfficeLocationRepository> _mockOfficeLocationRepository;

    [SetUp]
    public void Setup()
    {
        _mockOfficeLocationRepository = new Mock<IOfficeLocationRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new UpdateOfficeLocationCommandHandler(
            officeLocationRepository: _mockOfficeLocationRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task UpdateOfficeLocation_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnOfficeLocation = new OfficeLocation() { Id = 1, OfficeLocationName = "Test_1" };

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationAsync(It.IsAny<int>()))
            .ReturnsAsync(returnOfficeLocation);

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.UpdateOfficeLocationAsync(It.IsAny<OfficeLocation>()))
            .ReturnsAsync((OfficeLocation officeLocation) => officeLocation);

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.CheckIfOfficeLocationNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new UpdateOfficeLocationCommand(Id: 1, OfficeLocationName: "Test_2"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnOfficeLocation));
        _mockOfficeLocationRepository.Verify(
            m => m.GetOfficeLocationAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
        _mockOfficeLocationRepository.Verify(
            m =>
                m.UpdateOfficeLocationAsync(
                    It.Is<OfficeLocation>(officeLocation =>
                        officeLocation.Id == 1 && officeLocation.OfficeLocationName == "Test_2"
                    )
                ),
            Times.Once
        );
        _mockLogRepo.Verify(
            m =>
                m.AddOfficeLocationLogForCurrentActor(
                    It.IsAny<OfficeLocation>(),
                    Action.UPDATED_OFFICE_LOCATION,
                    It.Is<List<LogChange>>(changes =>
                        changes[0].Property == "OfficeLocationName"
                        && changes[0].NewValue == "Test_2"
                        && changes[0].OldValue == "Test_1"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task UpdateOfficeLocation_NoLogCreatedIfValuesAreEqual()
    {
        // Arrange
        var returnOfficeLocation = new OfficeLocation() { Id = 1, OfficeLocationName = "Test_1" };

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationAsync(It.IsAny<int>()))
            .ReturnsAsync(returnOfficeLocation);

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.UpdateOfficeLocationAsync(It.IsAny<OfficeLocation>()))
            .ReturnsAsync((OfficeLocation officeLocation) => officeLocation);

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.CheckIfOfficeLocationNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new UpdateOfficeLocationCommand(Id: 1, OfficeLocationName: "Test_1"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddOfficeLocationLogForCurrentActor(
                    It.IsAny<OfficeLocation>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }

    [Test]
    public void UpdateOfficeLocation_ThrowsOfficeLocationNameAlreadyExistsException_IfNewOfficeLocationNameAlreadyExists()
    {
        // Arrange
        var returnOfficeLocation = new OfficeLocation() { Id = 1, OfficeLocationName = "Test_1" };

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationAsync(It.IsAny<int>()))
            .ReturnsAsync(returnOfficeLocation);

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.UpdateOfficeLocationAsync(It.IsAny<OfficeLocation>()))
            .ReturnsAsync((OfficeLocation officeLocation) => officeLocation);

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.CheckIfOfficeLocationNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act + Assert
        var ex = Assert.ThrowsAsync<OfficeLocationNameAlreadyExistsException>(async () =>
            await _handler.Handle(
                new UpdateOfficeLocationCommand(Id: 1, OfficeLocationName: "Test_2"),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test_2"));
    }
}

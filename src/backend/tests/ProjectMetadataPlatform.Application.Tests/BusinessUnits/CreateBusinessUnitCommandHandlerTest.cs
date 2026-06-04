using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.BusinessUnits;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.BusinessUnits;

[TestFixture]
public class CreateBusinessUnitCommandHandlerTest
{
    private CreateBusinessUnitCommandHandler _handler;
    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();

        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateBusinessUnitCommandHandler(
            businessUnitRepository: _mockBusinessUnitRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task CreateBusinessUnit_NameDoesNotAlreadyExists_WorksFine()
    {
        // Arrange
        _mockBusinessUnitRepository
            .Setup(repo => repo.CheckIfBusinessUnitNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockBusinessUnitRepository
            .Setup(repo => repo.AddBusinessUnitAsync(It.IsAny<BusinessUnit>()))
            .Callback(
                (BusinessUnit businessUnitBeingAdded) =>
                {
                    businessUnitBeingAdded.Id = 1;
                }
            )
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(
            new CreateBusinessUnitCommand(BusinessUnitName: "Test Name"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.EqualTo(1));
        _mockLogRepo.Verify(
            m =>
                m.AddBusinessUnitLogForCurrentActor(
                    It.IsAny<BusinessUnit>(),
                    Action.ADDED_BUSINESS_UNIT,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockBusinessUnitRepository.Verify(
            m =>
                m.AddBusinessUnitAsync(
                    It.Is<BusinessUnit>(businessUnit =>
                        businessUnit.BusinessUnitName == "Test Name"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public void CreateBusinessUnit_NameAlreadyExists_ThrowsBusinessUnitNameAlreadyExistsException()
    {
        // Arrange
        _mockBusinessUnitRepository
            .Setup(repo => repo.CheckIfBusinessUnitNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<BusinessUnitNameAlreadyExistsException>(async () =>
            await _handler.Handle(
                new CreateBusinessUnitCommand(BusinessUnitName: "Test Name"),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test Name"));
    }
}

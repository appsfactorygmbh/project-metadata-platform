using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.BusinessUnits;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.Tests.BusinessUnits;

[TestFixture]
public class GetLinkedTeamsQueryHandlerTest
{
    private GetLinkedTeamsQueryHandler _handler;
    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;

    [SetUp]
    public void Setup()
    {
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _handler = new GetLinkedTeamsQueryHandler(
            businessUnitRepository: _mockBusinessUnitRepository.Object
        );
    }

    [Test]
    public async Task GetLinkedTeams_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnBusinessUnit = new BusinessUnit()
        {
            Id = 1,
            BusinessUnitName = "Test_1",
            Teams =
            [
                new()
                {
                    Id = 111,
                    TeamName = "teams",

                    BusinessUnitId = 1,
                },
                new()
                {
                    Id = 222,
                    TeamName = "teams",

                    BusinessUnitId = 1,
                },
            ],
        };

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitWithTeamsAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBusinessUnit);

        // Act
        var result = await _handler.Handle(
            new GetLinkedTeamsQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result, Does.Contain(111));
            Assert.That(result, Does.Contain(222));
        });
        _mockBusinessUnitRepository.Verify(
            m => m.GetBusinessUnitWithTeamsAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
    }
}

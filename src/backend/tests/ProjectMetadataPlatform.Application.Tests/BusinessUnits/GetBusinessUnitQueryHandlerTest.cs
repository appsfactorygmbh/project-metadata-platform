using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.BusinessUnits;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;

namespace ProjectMetadataPlatform.Application.Tests.BusinessUnits;

[TestFixture]
public class GetBusinessUnitQueryHandlerTest
{
    private GetBusinessUnitQueryHandler _handler;
    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;

    [SetUp]
    public void Setup()
    {
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _handler = new GetBusinessUnitQueryHandler(
            businessUnitRepository: _mockBusinessUnitRepository.Object
        );
    }

    [Test]
    public async Task GetBusinessUnit_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnBusinessUnit = new BusinessUnit() { Id = 1, BusinessUnitName = "Test_1" };

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBusinessUnit);

        // Act
        var result = await _handler.Handle(
            new GetBusinessUnitQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnBusinessUnit));
        _mockBusinessUnitRepository.Verify(
            m => m.GetBusinessUnitAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
    }

    [Test]
    public void GetBusinessUnit_ThrowBusinessUnitNotFoundException_IfBusinessUnitNotFound()
    {
        // Arrange
        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitAsync(It.IsAny<int>()))
            .ThrowsAsync(new BusinessUnitNotFoundException(1));

        // Act + Assert
        var ex = Assert.ThrowsAsync<BusinessUnitNotFoundException>(async () =>
            await _handler.Handle(new GetBusinessUnitQuery(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }
}

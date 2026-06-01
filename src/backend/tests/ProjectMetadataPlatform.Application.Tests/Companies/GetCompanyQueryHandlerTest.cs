using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Companies;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;

namespace ProjectMetadataPlatform.Application.Tests.Companies;

[TestFixture]
public class GetCompanyQueryHandlerTest
{
    private GetCompanyQueryHandler _handler;
    private Mock<ICompanyRepository> _mockCompanyRepository;

    [SetUp]
    public void Setup()
    {
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _handler = new GetCompanyQueryHandler(companyRepository: _mockCompanyRepository.Object);
    }

    [Test]
    public async Task GetCompany_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnCompany = new Company() { Id = 1, CompanyName = "Test_1" };

        _mockCompanyRepository
            .Setup(repo => repo.GetCompanyAsync(It.IsAny<int>()))
            .ReturnsAsync(returnCompany);

        // Act
        var result = await _handler.Handle(
            new GetCompanyQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnCompany));
        _mockCompanyRepository.Verify(
            m => m.GetCompanyAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
    }

    [Test]
    public void GetCompany_ThrowCompanyNotFoundException_IfCompanyNotFound()
    {
        // Arrange
        _mockCompanyRepository
            .Setup(repo => repo.GetCompanyAsync(It.IsAny<int>()))
            .ThrowsAsync(new CompanyNotFoundException(1));

        // Act + Assert
        var ex = Assert.ThrowsAsync<CompanyNotFoundException>(async () =>
            await _handler.Handle(new GetCompanyQuery(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }
}

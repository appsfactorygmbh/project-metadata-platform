using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Companies;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Tests.Companies;

[TestFixture]
public class GetAllCompaniesQueryHandlerTest
{
    private GetAllCompaniesQueryHandler _handler;
    private Mock<ICompanyRepository> _mockCompanyRepository;

    [SetUp]
    public void Setup()
    {
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _handler = new GetAllCompaniesQueryHandler(
            companyRepository: _mockCompanyRepository.Object
        );
    }

    [Test]
    public async Task GetAllCompanies_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnCompany = new Company() { Id = 1, CompanyName = "Test_1" };

        _mockCompanyRepository
            .Setup(repo => repo.GetCompaniesAsync())
            .ReturnsAsync([returnCompany]);

        // Act
        var result = await _handler.Handle(
            new GetAllCompaniesQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo(returnCompany));
        _mockCompanyRepository.Verify(m => m.GetCompaniesAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllCompanies_ReturnsInOrder()
    {
        // Arrange
        List<Company> returnCompany =
        [
            new() { Id = 1, CompanyName = "Test_1" },
            new() { Id = 3, CompanyName = "test_3" },
            new() { Id = 2, CompanyName = "TesT_2" },
            new() { Id = 4, CompanyName = "Foo_2" },
        ];

        _mockCompanyRepository.Setup(repo => repo.GetCompaniesAsync()).ReturnsAsync(returnCompany);

        // Act
        var result = await _handler.Handle(
            new GetAllCompaniesQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        var resultList = result.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(resultList[0], Is.EqualTo(returnCompany[3]));
            Assert.That(resultList[1], Is.EqualTo(returnCompany[0]));
            Assert.That(resultList[2], Is.EqualTo(returnCompany[2]));
            Assert.That(resultList[3], Is.EqualTo(returnCompany[1]));
        });
    }
}

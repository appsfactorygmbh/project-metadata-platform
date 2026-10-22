using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Companies;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Tests.Companies;

[TestFixture]
public class GetAllCompaniesQueryHandlerTest
{
    private GetAllCompaniesQueryHandler _handler;
    private Mock<ICompanyRepository> _mockCompanyRepository;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IPaginationHelper> _paginationHelperMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelperMock = new Mock<IPaginationHelper>();
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _handler = new GetAllCompaniesQueryHandler(
            companyRepository: _mockCompanyRepository.Object,
            authorizationService: _authorizationServiceMock.Object,
            _paginationHelperMock.Object
        );
    }

    [Test]
    public async Task GetAllCompanies_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnCompany = new Company() { Id = 1, CompanyName = "Test_1" };

        _ = _mockCompanyRepository
            .Setup(repo => repo.GetCompaniesAsync(null))
            .ReturnsAsync(new List<Company> { returnCompany }.BuildMock());
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Company>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Company> query, int? limiz) => (query, null));
        // Act
        var result = await _handler.Handle(
            new GetAllCompaniesQuery(null, null),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1.Count, Is.EqualTo(1));
        Assert.That(result.Item1.First(), Is.EqualTo(returnCompany));
        _mockCompanyRepository.Verify(m => m.GetCompaniesAsync(null), Times.Once);
    }
}

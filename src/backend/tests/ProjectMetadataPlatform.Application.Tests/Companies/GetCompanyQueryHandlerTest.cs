using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Companies;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;

namespace ProjectMetadataPlatform.Application.Tests.Companies;

[TestFixture]
public class GetCompanyQueryHandlerTest
{
    private GetCompanyQueryHandler _handler;
    private Mock<ICompanyRepository> _mockCompanyRepository;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _handler = new GetCompanyQueryHandler(
            companyRepository: _mockCompanyRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetCompany_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnCompany = new Company() { Id = 1, CompanyName = "Test_1" };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Company>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.GET, true },
                }
            );
        _ = _mockCompanyRepository
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
        var returnCompany = new Company() { Id = 1, CompanyName = "Test_1" };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Company>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.GET, true },
                }
            );
        _ = _mockCompanyRepository
            .Setup(repo => repo.GetCompanyAsync(It.IsAny<int>()))
            .ThrowsAsync(new CompanyNotFoundException(1));

        // Act + Assert
        var ex = Assert.ThrowsAsync<CompanyNotFoundException>(async () =>
            await _handler.Handle(new GetCompanyQuery(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task GetCompany_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Company>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.GET, false },
                }
            );

        var request = new GetCompanyQuery(Id: 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

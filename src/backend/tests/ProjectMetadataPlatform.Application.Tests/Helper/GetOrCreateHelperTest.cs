using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Helper;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.Tests.Helper;

[TestFixture]
public class GetOrCreateHelperTest
{
    private Mock<IOfficeLocationRepository> _mockOfficeLocationRepository;

    private Mock<ICompanyRepository> _mockCompanyRepository;

    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;
    private Mock<IDepartmentRepository> _mockDepartmentRepository;
    private Mock<ILogRepository> _mockLogRepository;

    private Mock<IApiTokenRepository> _mockTokenRepository;
    private IGetOrCreateHelper _getOrCreateHelper;

    private Mock<IHttpContextAccessor> _httpContextAccessorMock;

    [SetUp]
    public void Setup()
    {
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _mockOfficeLocationRepository = new Mock<IOfficeLocationRepository>();
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _mockLogRepository = new Mock<ILogRepository>();
        _mockTokenRepository = new Mock<IApiTokenRepository>();

        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _getOrCreateHelper = new GetOrCreateHelper(
            _mockDepartmentRepository.Object,
            _mockBusinessUnitRepository.Object,
            _mockOfficeLocationRepository.Object,
            _mockCompanyRepository.Object,
            _mockLogRepository.Object,
            _mockTokenRepository.Object,
            _httpContextAccessorMock.Object
        );
    }

    [Test]
    public async Task GetOrCreateDepartment_DepartmentExistsTest()
    {
        var name = "A Name";
        var department = new Department { DepartmentName = name };
        _mockDepartmentRepository
            .Setup(m => m.CheckIfDepartmentNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockDepartmentRepository
            .Setup(m => m.GetDepartmentByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(department);

        var result = await _getOrCreateHelper.GetOrCreateDepartment(name);
        Assert.That(result, Is.EqualTo(department));

        _mockDepartmentRepository.Verify(
            m => m.AddDepartmentAsync(It.Is<Department>(d => d.DepartmentName == name)),
            Times.Never
        );
    }

    [Test]
    public async Task GetOrCreateDepartment_DepartmentDoesntExistNoScimTokenTest()
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.AuthenticationMethod, "JWT Token"),
            ],
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = contextUser };
        _ = _httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext)
            .Returns(httpContext);
        var name = "A Name";
        _mockDepartmentRepository
            .Setup(m => m.CheckIfDepartmentNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        Assert.ThrowsAsync<DepartmentNotFoundException>(() =>
            _getOrCreateHelper.GetOrCreateDepartment(name)
        );

        _mockDepartmentRepository.Verify(
            m => m.AddDepartmentAsync(It.Is<Department>(d => d.DepartmentName == name)),
            Times.Never
        );
    }

    [Test]
    public async Task GetOrCreateDepartment_DepartmentDoesntExistGetsCreatedTest()
    {
        var name = "A Name";
        var department = new Department { DepartmentName = name };

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, "camo"),
                new Claim(ClaimTypes.AuthenticationMethod, "API Token"),
            ],
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = contextUser };
        _ = _httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext)
            .Returns(httpContext);

        _mockDepartmentRepository
            .Setup(m => m.CheckIfDepartmentNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _mockTokenRepository.Setup(m => m.IsScimToken(It.IsAny<string>())).ReturnsAsync(true);

        var result = await _getOrCreateHelper.GetOrCreateDepartment(name);
        Assert.That(result.DepartmentName, Is.EqualTo(department.DepartmentName));

        _mockDepartmentRepository.Verify(
            m => m.AddDepartmentAsync(It.Is<Department>(d => d.DepartmentName == name)),
            Times.Once
        );
    }

        [Test]
    public async Task GetOrCreateCompany_CompanyExistsTest()
    {
        var name = "A Name";
        var company = new Company { CompanyName = name };
        _mockCompanyRepository
            .Setup(m => m.CheckIfCompanyNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockCompanyRepository
            .Setup(m => m.GetCompanyByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(company);

        var result = await _getOrCreateHelper.GetOrCreateCompany(name);
        Assert.That(result, Is.EqualTo(company));

        _mockCompanyRepository.Verify(
            m => m.AddCompanyAsync(It.Is<Company>(d => d.CompanyName == name)),
            Times.Never
        );
    }

    [Test]
    public async Task GetOrCreateCompany_CompanyDoesntExistNoScimTokenTest()
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.AuthenticationMethod, "JWT Token"),
            ],
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = contextUser };
        _ = _httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext)
            .Returns(httpContext);
        var name = "A Name";
        _mockCompanyRepository
            .Setup(m => m.CheckIfCompanyNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        Assert.ThrowsAsync<CompanyNotFoundException>(() =>
            _getOrCreateHelper.GetOrCreateCompany(name)
        );

        _mockCompanyRepository.Verify(
            m => m.AddCompanyAsync(It.Is<Company>(d => d.CompanyName == name)),
            Times.Never
        );
    }

    [Test]
    public async Task GetOrCreateCompany_CompanyDoesntExistGetsCreatedTest()
    {
        var name = "A Name";
        var company = new Company { CompanyName = name };

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, "camo"),
                new Claim(ClaimTypes.AuthenticationMethod, "API Token"),
            ],
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = contextUser };
        _ = _httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext)
            .Returns(httpContext);

        _mockCompanyRepository
            .Setup(m => m.CheckIfCompanyNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _mockTokenRepository.Setup(m => m.IsScimToken(It.IsAny<string>())).ReturnsAsync(true);

        var result = await _getOrCreateHelper.GetOrCreateCompany(name);
        Assert.That(result.CompanyName, Is.EqualTo(company.CompanyName));

        _mockCompanyRepository.Verify(
            m => m.AddCompanyAsync(It.Is<Company>(d => d.CompanyName == name)),
            Times.Once
        );
    }

        [Test]
    public async Task GetOrCreateOfficeLocation_OfficeLocationExistsTest()
    {
        var name = "A Name";
        var officeLocation = new OfficeLocation { OfficeLocationName = name };
        _mockOfficeLocationRepository
            .Setup(m => m.CheckIfOfficeLocationNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockOfficeLocationRepository
            .Setup(m => m.GetOfficeLocationByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(officeLocation);

        var result = await _getOrCreateHelper.GetOrCreateOfficeLocation(name);
        Assert.That(result, Is.EqualTo(officeLocation));

        _mockOfficeLocationRepository.Verify(
            m => m.AddOfficeLocationAsync(It.Is<OfficeLocation>(d => d.OfficeLocationName == name)),
            Times.Never
        );
    }

    [Test]
    public async Task GetOrCreateOfficeLocation_OfficeLocationDoesntExistNoScimTokenTest()
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.AuthenticationMethod, "JWT Token"),
            ],
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = contextUser };
        _ = _httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext)
            .Returns(httpContext);
        var name = "A Name";
        _mockOfficeLocationRepository
            .Setup(m => m.CheckIfOfficeLocationNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        Assert.ThrowsAsync<OfficeLocationNotFoundException>(() =>
            _getOrCreateHelper.GetOrCreateOfficeLocation(name)
        );

        _mockOfficeLocationRepository.Verify(
            m => m.AddOfficeLocationAsync(It.Is<OfficeLocation>(d => d.OfficeLocationName == name)),
            Times.Never
        );
    }

    [Test]
    public async Task GetOrCreateOfficeLocation_OfficeLocationDoesntExistGetsCreatedTest()
    {
        var name = "A Name";
        var officeLocation = new OfficeLocation { OfficeLocationName = name };

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, "camo"),
                new Claim(ClaimTypes.AuthenticationMethod, "API Token"),
            ],
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = contextUser };
        _ = _httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext)
            .Returns(httpContext);

        _mockOfficeLocationRepository
            .Setup(m => m.CheckIfOfficeLocationNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _mockTokenRepository.Setup(m => m.IsScimToken(It.IsAny<string>())).ReturnsAsync(true);

        var result = await _getOrCreateHelper.GetOrCreateOfficeLocation(name);
        Assert.That(result.OfficeLocationName, Is.EqualTo(officeLocation.OfficeLocationName));

        _mockOfficeLocationRepository.Verify(
            m => m.AddOfficeLocationAsync(It.Is<OfficeLocation>(d => d.OfficeLocationName == name)),
            Times.Once
        );
    }

        [Test]
    public async Task GetOrCreateBusinessUnit_BusinessUnitExistsTest()
    {
        var name = "A Name";
        var businessUnit = new BusinessUnit { BusinessUnitName = name };
        _mockBusinessUnitRepository
            .Setup(m => m.CheckIfBusinessUnitNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockBusinessUnitRepository
            .Setup(m => m.GetBusinessUnitByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(businessUnit);

        var result = await _getOrCreateHelper.GetOrCreateBusinessUnit(name);
        Assert.That(result, Is.EqualTo(businessUnit));

        _mockBusinessUnitRepository.Verify(
            m => m.AddBusinessUnitAsync(It.Is<BusinessUnit>(d => d.BusinessUnitName == name)),
            Times.Never
        );
    }

    [Test]
    public async Task GetOrCreateBusinessUnit_BusinessUnitDoesntExistNoScimTokenTest()
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.AuthenticationMethod, "JWT Token"),
            ],
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = contextUser };
        _ = _httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext)
            .Returns(httpContext);
        var name = "A Name";
        _mockBusinessUnitRepository
            .Setup(m => m.CheckIfBusinessUnitNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        Assert.ThrowsAsync<BusinessUnitNotFoundException>(() =>
            _getOrCreateHelper.GetOrCreateBusinessUnit(name)
        );

        _mockBusinessUnitRepository.Verify(
            m => m.AddBusinessUnitAsync(It.Is<BusinessUnit>(d => d.BusinessUnitName == name)),
            Times.Never
        );
    }

    [Test]
    public async Task GetOrCreateBusinessUnit_BusinessUnitDoesntExistGetsCreatedTest()
    {
        var name = "A Name";
        var businessUnit = new BusinessUnit { BusinessUnitName = name };

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, "camo"),
                new Claim(ClaimTypes.AuthenticationMethod, "API Token"),
            ],
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = contextUser };
        _ = _httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext)
            .Returns(httpContext);

        _mockBusinessUnitRepository
            .Setup(m => m.CheckIfBusinessUnitNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _mockTokenRepository.Setup(m => m.IsScimToken(It.IsAny<string>())).ReturnsAsync(true);

        var result = await _getOrCreateHelper.GetOrCreateBusinessUnit(name);
        Assert.That(result.BusinessUnitName, Is.EqualTo(businessUnit.BusinessUnitName));

        _mockBusinessUnitRepository.Verify(
            m => m.AddBusinessUnitAsync(It.Is<BusinessUnit>(d => d.BusinessUnitName == name)),
            Times.Once
        );
    }
}

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
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Companies;

[TestFixture]
public class CreateCompanyCommandHandlerTest
{
    private CreateCompanyCommandHandler _handler;
    private Mock<ICompanyRepository> _mockCompanyRepository;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateCompanyCommandHandler(
            companyRepository: _mockCompanyRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task CreateCompany_NameDoesNotAlreadyExists_WorksFine()
    {
        // Arrange
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
                    { AuthorizationConstants.Actions.CREATE, true },
                }
            );
        _ = _mockCompanyRepository
            .Setup(repo => repo.CheckIfCompanyNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _ = _mockCompanyRepository
            .Setup(repo => repo.AddCompanyAsync(It.IsAny<Company>()))
            .Callback(
                (Company companyBeingAdded) =>
                {
                    companyBeingAdded.Id = 1;
                }
            )
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(
            new CreateCompanyCommand(CompanyName: "Test Name"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.EqualTo(1));
        _mockLogRepo.Verify(
            m =>
                m.AddCompanyLogForCurrentActor(
                    It.IsAny<Company>(),
                    Action.ADDED_COMPANY,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockCompanyRepository.Verify(
            m => m.AddCompanyAsync(It.Is<Company>(company => company.CompanyName == "Test Name")),
            Times.Once
        );
    }

    [Test]
    public void CreateCompany_NameAlreadyExists_ThrowsCompanyNameAlreadyExistsException()
    {
        // Arrange
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
                    { AuthorizationConstants.Actions.CREATE, true },
                }
            );
        _ = _mockCompanyRepository
            .Setup(repo => repo.CheckIfCompanyNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<CompanyNameAlreadyExistsException>(async () =>
            await _handler.Handle(
                new CreateCompanyCommand(CompanyName: "Test Name"),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test Name"));
    }

    [Test]
    public async Task CreateCompanyCommand_AuthorizationFailsThrowsTest()
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
                    { AuthorizationConstants.Actions.CREATE, false },
                }
            );

        var request = new CreateCompanyCommand(CompanyName: "Test Name");

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}

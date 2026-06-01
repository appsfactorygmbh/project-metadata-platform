using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Companies;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Companies;

[TestFixture]
public class CreateCompanyCommandHandlerTest
{
    private CreateCompanyCommandHandler _handler;
    private Mock<ICompanyRepository> _mockCompanyRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockCompanyRepository = new Mock<ICompanyRepository>();

        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateCompanyCommandHandler(
            companyRepository: _mockCompanyRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task CreateCompany_NameDoesNotAlreadyExists_WorksFine()
    {
        // Arrange
        _mockCompanyRepository
            .Setup(repo => repo.CheckIfCompanyNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockCompanyRepository
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
        _mockCompanyRepository
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
}

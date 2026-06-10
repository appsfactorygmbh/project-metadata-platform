using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Companies;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Tests.Companies;

[TestFixture]
public class UpdateCompanyCommandHandlerTest
{
    private UpdateCompanyCommandHandler _handler;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    private Mock<ICompanyRepository> _mockCompanyRepository;

    [SetUp]
    public void Setup()
    {
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new UpdateCompanyCommandHandler(
            companyRepository: _mockCompanyRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task UpdateCompany_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnCompany = new Company() { Id = 1, CompanyName = "Test_1" };

        _ = _mockCompanyRepository
            .Setup(repo => repo.GetCompanyAsync(It.IsAny<int>()))
            .ReturnsAsync(returnCompany);

        _ = _mockCompanyRepository
            .Setup(repo => repo.UpdateCompanyAsync(It.IsAny<Company>()))
            .ReturnsAsync((Company company) => company);

        _ = _mockCompanyRepository
            .Setup(repo => repo.CheckIfCompanyNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new UpdateCompanyCommand(Id: 1, CompanyName: "Test_2"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnCompany));
        _mockCompanyRepository.Verify(
            m => m.GetCompanyAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
        _mockCompanyRepository.Verify(
            m =>
                m.UpdateCompanyAsync(
                    It.Is<Company>(company => company.Id == 1 && company.CompanyName == "Test_2")
                ),
            Times.Once
        );
        _mockLogRepo.Verify(
            m =>
                m.AddCompanyLogForCurrentActor(
                    It.IsAny<Company>(),
                    Action.UPDATED_COMPANY,
                    It.Is<List<LogChange>>(changes =>
                        changes[0].Property == "CompanyName"
                        && changes[0].NewValue == "Test_2"
                        && changes[0].OldValue == "Test_1"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task UpdateCompany_NoLogCreatedIfValuesAreEqual()
    {
        // Arrange
        var returnCompany = new Company() { Id = 1, CompanyName = "Test_1" };

        _ = _mockCompanyRepository
            .Setup(repo => repo.GetCompanyAsync(It.IsAny<int>()))
            .ReturnsAsync(returnCompany);

        _ = _mockCompanyRepository
            .Setup(repo => repo.UpdateCompanyAsync(It.IsAny<Company>()))
            .ReturnsAsync((Company company) => company);

        _ = _mockCompanyRepository
            .Setup(repo => repo.CheckIfCompanyNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new UpdateCompanyCommand(Id: 1, CompanyName: "Test_1"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddCompanyLogForCurrentActor(
                    It.IsAny<Company>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }

    [Test]
    public void UpdateCompany_ThrowsCompanyNameAlreadyExistsException_IfNewCompanyNameAlreadyExists()
    {
        // Arrange
        var returnCompany = new Company() { Id = 1, CompanyName = "Test_1" };

        _ = _mockCompanyRepository
            .Setup(repo => repo.GetCompanyAsync(It.IsAny<int>()))
            .ReturnsAsync(returnCompany);

        _ = _mockCompanyRepository
            .Setup(repo => repo.UpdateCompanyAsync(It.IsAny<Company>()))
            .ReturnsAsync((Company company) => company);

        _ = _mockCompanyRepository
            .Setup(repo => repo.CheckIfCompanyNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act + Assert
        var ex = Assert.ThrowsAsync<CompanyNameAlreadyExistsException>(async () =>
            await _handler.Handle(
                new UpdateCompanyCommand(Id: 1, CompanyName: "Test_2"),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test_2"));
    }
}

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
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Companies;

[TestFixture]
public class DeleteCompanyCommandHandlerTest
{
    private DeleteCompanyCommandHandler _handler;
    private Mock<ICompanyRepository> _mockCompanyRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteCompanyCommandHandler(
            companyRepository: _mockCompanyRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task DeleteCompany_NoLinkedProjects_WorksFine()
    {
        // Arrange
        var returnCompany = new Company()
        {
            Id = 1,
            CompanyName = "Test_1",
            Projects = [],
        };

        _ = _mockCompanyRepository
            .Setup(repo => repo.GetCompanyWithProjectsAsync(It.IsAny<int>()))
            .ReturnsAsync(returnCompany);

        // Act
        await _handler.Handle(new DeleteCompanyCommand(Id: 1), It.IsAny<CancellationToken>());

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddCompanyLogForCurrentActor(
                    It.IsAny<Company>(),
                    Action.REMOVED_COMPANY,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockCompanyRepository.Verify(
            m =>
                m.DeleteCompanyAsync(
                    It.Is<Company>(company => company.Id == 1 && company.CompanyName == "Test_1")
                ),
            Times.Once
        );
    }

    [Test]
    public void DeleteCompany_StillLinkedProjects_ThrowsCompanyStillLinkedToProjectsException()
    {
        // Arrange
        _ = _mockCompanyRepository
            .Setup(repo => repo.CheckIfCompanyNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var returnCompany = new Company()
        {
            Id = 1,
            CompanyName = "Test_1",
            Projects =
            [
                new()
                {
                    Id = 111,
                    ProjectName = "Projects",
                    Slug = "project",
                    ClientName = "Project Client",
                    CompanyId = 1,
                },
            ],
        };

        _ = _mockCompanyRepository
            .Setup(repo => repo.GetCompanyWithProjectsAsync(It.IsAny<int>()))
            .ReturnsAsync(returnCompany);

        // Act + Assert
        var ex = Assert.ThrowsAsync<CompanyStillLinkedToProjectsException>(async () =>
            await _handler.Handle(new DeleteCompanyCommand(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("111"));
    }
}

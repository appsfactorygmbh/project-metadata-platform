using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Helper;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

namespace ProjectMetadataPlatform.Application.Tests.Helper;

public class SlugHelperTest
{
    private SlugHelper _slugHelper;
    private Mock<IProjectsRepository> _mockProjectsRepository;

    [SetUp]
    public void Setup()
    {
        _mockProjectsRepository = new Mock<IProjectsRepository>();
        _slugHelper = new SlugHelper(_mockProjectsRepository.Object);
    }

    [Test]
    public void GenerateSlug_Test()
    {
        const string input = "Example Project Version 3-14-1;";
        const string expected = "example_project_version_3141";

        var result = _slugHelper.GenerateSlug(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GenerateSlug_Test_ToLowerCase()
    {
        const string input = "ExampleProject";
        const string expected = "exampleproject";

        var result = _slugHelper.GenerateSlug(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GenerateSlug_Test_ReplaceWhitespaces()
    {
        const string input = "Example Project";
        const string expected = "example_project";

        var result = _slugHelper.GenerateSlug(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GenerateSlug_Test_TrimWhitespaces()
    {
        const string input = " ExampleProject ";
        const string expected = "exampleproject";

        var result = _slugHelper.GenerateSlug(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GenerateSlug_Test_ReplaceSpecialChars()
    {
        const string input = "ExampleProject!§$%&/();:-";
        const string expected = "exampleproject";

        var result = _slugHelper.GenerateSlug(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetProjectIdBySlug_Test()
    {
        const string slug = "example_project";
        _mockProjectsRepository
            .Setup(m => m.GetProjectIdBySlugAsync(It.IsAny<string>()))
            .ReturnsAsync(1);

        var result = await _slugHelper.GetProjectIdBySlug(slug);

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void GetProjectIdBySlug_Test_ThrowsException()
    {
        const string slug = "example_project";

        _mockProjectsRepository
            .Setup(m => m.GetProjectIdBySlugAsync(It.IsAny<string>()))
            .ThrowsAsync(new ProjectNotFoundException("Project not found"));

        Assert.ThrowsAsync<ProjectNotFoundException>(() => _slugHelper.GetProjectIdBySlug(slug));
    }

    [Test]
    public async Task ProjectSlugExists_Test()
    {
        const string slug = "example_project";
        _mockProjectsRepository
            .Setup(m => m.GetProjectIdBySlugAsync(It.IsAny<string>()))
            .ReturnsAsync(1);

        var result = await _slugHelper.CheckProjectSlugExists(slug);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ProjectSlugExists_Test_ReturnsFalse()
    {
        const string slug = "example_project";
        _mockProjectsRepository
            .Setup(m => m.GetProjectIdBySlugAsync(It.IsAny<string>()))
            .ThrowsAsync(new ProjectNotFoundException("Project not found"));

        var result = await _slugHelper.CheckProjectSlugExists(slug);

        Assert.That(result, Is.False);
    }
}

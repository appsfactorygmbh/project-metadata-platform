using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Tests;

[TestFixture]
public class DependencyInjectionTests
{
    private Mock<IProjectsRepository> _mockProjectsRepository;

    [SetUp]
    public void Setup()
    {
        _mockProjectsRepository = new Mock<IProjectsRepository>();
    }

    [Test]
    public void RequestHandlersAreRegistered()
    {
        var serviceCollection = new ServiceCollection() as IServiceCollection;

        serviceCollection.AddSingleton(_mockProjectsRepository.Object);

        serviceCollection.AddApplicationDependencies();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<ISlugHelper>(), Is.Not.Null);

        serviceCollection.BuildServiceProvider();
    }
}

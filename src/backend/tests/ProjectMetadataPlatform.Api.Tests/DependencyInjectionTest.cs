using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

namespace ProjectMetadataPlatform.Api.Tests;

[TestFixture]
public class DependencyInjectionTests
{
    [Test]
    public void RequestHandlersAreRegistered()
    {
        var serviceCollection = new ServiceCollection() as IServiceCollection;

        serviceCollection.AddApiDependencies();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        Assert.Multiple(() =>
        {
            Assert.That(serviceProvider.GetService<ILogConverter>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<IExceptionHandler<PmpException>>(), Is.Not.Null);
            Assert.That(
                serviceProvider.GetService<IExceptionHandler<ProjectException>>(),
                Is.Not.Null
            );
            Assert.That(serviceProvider.GetService<IExceptionHandler<LogException>>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<IExceptionHandler<LogException>>(), Is.Not.Null);
            Assert.That(
                serviceProvider.GetService<IExceptionHandler<AuthException>>(),
                Is.Not.Null
            );
        });
    }
}

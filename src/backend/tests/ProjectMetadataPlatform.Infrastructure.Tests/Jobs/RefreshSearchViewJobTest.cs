using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Jobs;
using Quartz;

namespace ProjectMetadataPlatform.Infrastructure.Tests.Jobs;

[TestFixture]
public class RefreshSearchViewJobTest : TestsWithDatabase
{
    private RefreshSearchViewJob _job;
    private ProjectMetadataPlatformDbContext _context;

    private Mock<IJobExecutionContext> _jobContext;
    private Mock<ILogger<RefreshSearchViewJob>> _logger;

    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _jobContext = new Mock<IJobExecutionContext>();
        _logger = new Mock<ILogger<RefreshSearchViewJob>>();

        _job = new RefreshSearchViewJob(_context, _logger.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public async Task RefreshSearchViewJob_Success_Test()
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            await _job.Execute(_jobContext.Object, CancellationToken.None);
        });
    }

    [Test]
    public void RefreshSearchViewJob_Failure_Test()
    {
        _context.Dispose();

        var exception = Assert.ThrowsAsync<JobExecutionException>(async () =>
        {
            await _job.Execute(_jobContext.Object, CancellationToken.None);
        });

        Assert.That(exception.InnerException, Is.InstanceOf<ObjectDisposedException>());
    }
}

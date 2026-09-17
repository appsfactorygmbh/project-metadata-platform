using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Infrastructure.Jobs;
using Quartz;

namespace ProjectMetadataPlatform.Infrastructure.Tests.Jobs;

[TestFixture]
public class RefreshTokenCleanUpJobTest
{
    private RefreshTokenCleanUpJob _job;
    private Mock<IMediator> _mediator;

    private Mock<IJobExecutionContext> _context;
    private Mock<ILogger<RefreshTokenCleanUpJob>> _logger;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _context = new Mock<IJobExecutionContext>();
        _logger = new Mock<ILogger<RefreshTokenCleanUpJob>>();

        _job = new RefreshTokenCleanUpJob(_mediator.Object, _logger.Object);
    }

    [Test]
    public async Task RefreshTokenCleanUpJob_Success_Test()
    {
        await _job.Execute(_context.Object, CancellationToken.None);
        _mediator.Verify(m => m.Send(It.IsAny<CleanUpRefreshTokensCommand>()), Times.Once);
    }

    [Test]
    public async Task RefreshTokenCleanUpJob_Failure_Test()
    {
        _mediator
            .Setup(m => m.Send(It.IsAny<CleanUpRefreshTokensCommand>()))
            .ThrowsAsync(new Exception("this is a Exception"));
        _ = Assert.ThrowsAsync<JobExecutionException>(() =>
            _job.Execute(_context.Object, CancellationToken.None).AsTask()
        );
    }
}

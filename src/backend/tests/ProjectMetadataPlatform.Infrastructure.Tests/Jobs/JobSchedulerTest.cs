using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Infrastructure.Jobs;
using Quartz;

namespace ProjectMetadataPlatform.Infrastructure.Tests.Jobs;

[TestFixture]
public class JobSchedulerTest
{
    private JobScheduler _jobScheduler;
    private Mock<ISchedulerFactory> _mockSchedulerFactory;
    private Mock<IScheduler> _mockScheduler;
    private Mock<TimeProvider> _mockTimeProvider;

    [SetUp]
    public void Setup()
    {
        _mockSchedulerFactory = new Mock<ISchedulerFactory>();
        _mockScheduler = new Mock<IScheduler>();
        _mockSchedulerFactory
            .Setup(m => m.GetScheduler(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_mockScheduler.Object);
        _mockTimeProvider = new Mock<TimeProvider>();
        _jobScheduler = new JobScheduler(_mockSchedulerFactory.Object, _mockTimeProvider.Object);
    }

    [Test]
    public async Task ScheduleSearchViewRefreshAsync_TriggerExists_NothingScheduled()
    {
        _mockScheduler.Setup(m => m.Exists(It.IsAny<TriggerKey>())).ReturnsAsync(true);

        await _jobScheduler.ScheduleSearchViewRefreshAsync();
        _mockScheduler.Verify(m => m.ScheduleJob(It.IsAny<ITrigger>()), Times.Never);
    }

    [Test]
    public async Task ScheduleSearchViewRefreshAsync_SchedulesWithTrigger()
    {
        _mockScheduler.Setup(m => m.Exists(It.IsAny<TriggerKey>())).ReturnsAsync(false);
        _mockTimeProvider.Setup(m => m.GetUtcNow()).Returns(new DateTimeOffset());
        await _jobScheduler.ScheduleSearchViewRefreshAsync();
        _mockScheduler.Verify(
            m =>
                m.ScheduleJob(
                    It.Is<ITrigger>(t =>
                        t.Key.Name == "RefreshSearchViewJob"
                        && t.JobKey.Name == "RefreshSearchViewJob"
                        && t.FinalFireTimeUtc == new DateTimeOffset().AddSeconds(5)
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task ScheduleSearchViewRefreshAsync_ObjectAlreadyExistsException_DoesNotThrow()
    {
        _mockScheduler.Setup(m => m.Exists(It.IsAny<TriggerKey>())).ReturnsAsync(false);
        _mockTimeProvider.Setup(m => m.GetUtcNow()).Returns(new DateTimeOffset());
        _mockScheduler
            .Setup(m => m.ScheduleJob(It.IsAny<ITrigger>()))
            .ThrowsAsync(new ObjectAlreadyExistsException(""));
        await Assert.DoesNotThrowAsync(_jobScheduler.ScheduleSearchViewRefreshAsync);
        _mockScheduler.Verify(
            m =>
                m.ScheduleJob(
                    It.Is<ITrigger>(t =>
                        t.Key.Name == "RefreshSearchViewJob"
                        && t.JobKey.Name == "RefreshSearchViewJob"
                        && t.FinalFireTimeUtc == new DateTimeOffset().AddSeconds(5)
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task ScheduleSearchViewRefreshAsync_Exception_Throws()
    {
        _mockScheduler.Setup(m => m.Exists(It.IsAny<TriggerKey>())).ReturnsAsync(false);
        _mockTimeProvider.Setup(m => m.GetUtcNow()).Returns(new DateTimeOffset());
        _mockScheduler.Setup(m => m.ScheduleJob(It.IsAny<ITrigger>())).ThrowsAsync(new Exception());
        await Assert.ThrowsAsync<Exception>(_jobScheduler.ScheduleSearchViewRefreshAsync);
        _mockScheduler.Verify(
            m =>
                m.ScheduleJob(
                    It.Is<ITrigger>(t =>
                        t.Key.Name == "RefreshSearchViewJob"
                        && t.JobKey.Name == "RefreshSearchViewJob"
                        && t.FinalFireTimeUtc == new DateTimeOffset().AddSeconds(5)
                    )
                ),
            Times.Once
        );
    }
}

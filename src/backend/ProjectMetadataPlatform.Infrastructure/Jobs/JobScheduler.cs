using System;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using Quartz;

namespace ProjectMetadataPlatform.Infrastructure.Jobs;

/// <summary>
/// Implements <see cref="IJobScheduler"/>
/// </summary>
public class JobScheduler : IJobScheduler
{
    private readonly ISchedulerFactory _schedulerFactory;

    /// <summary>
    /// Constructor for <see cref="JobScheduler"/>
    /// </summary>
    /// <param name="schedulerFactory"></param>
    public JobScheduler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    /// <inheritdoc/>
    public async Task ScheduleSearchViewRefreshAsync()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var triggerKey = new TriggerKey("RefreshSearchViewJob");

        if (!await scheduler.Exists(triggerKey))
        {
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(triggerKey)
                .ForJob(new JobKey("RefreshSearchViewJob"))
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(5))
                .Build();

            try
            {
                _ = await scheduler.ScheduleJob(trigger);
            }
            catch (ObjectAlreadyExistsException) { }
        }
    }
}

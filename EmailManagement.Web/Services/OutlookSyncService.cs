using EmailManagement.Web.Pages;
using Quartz;
using System.Reflection.Metadata;

namespace EmailManagement.Web.Services
{
    public class OutlookSyncService : IHostedService
    {
        private readonly IMessageSynchronizer _messageSynchronizer;
        private readonly IScheduler _scheduler;

        public OutlookSyncService(IMessageSynchronizer messageSynchronizer, IScheduler scheduler)
        {
            _messageSynchronizer = messageSynchronizer;
            _scheduler = scheduler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var job = JobBuilder.Create<SyncMessagesJob>()
              .WithIdentity("outlookSyncJob")
              .Build();

            var trigger = TriggerBuilder.Create()
              .WithIdentity("outlookSyncTrigger")
              .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(60) 
                .RepeatForever())
              .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _scheduler?.Shutdown(true);
            return Task.CompletedTask;
        }
    }

    public class SyncMessagesJob : IJob
    {
        private readonly IMessageSynchronizer _messageSynchronizer;

        public SyncMessagesJob(IMessageSynchronizer messageSynchronizer)
        {
            _messageSynchronizer = messageSynchronizer;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var lastRunTime = context.PreviousFireTimeUtc.HasValue ?
                context.PreviousFireTimeUtc.Value :
                DateTimeOffset.UtcNow;
            await _messageSynchronizer.SyncOutlookMessages(lastRunTime);
        }
    }


}

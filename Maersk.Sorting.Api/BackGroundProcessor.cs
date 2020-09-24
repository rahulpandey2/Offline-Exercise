using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class BackGroundProcessor : IBackGroundProcessor, IHostedService
    {
        private readonly ISortJobProcessor _sortJobProcessor;
        private bool _runBackGroundProcess = true;
        public BackGroundProcessor(ISortJobProcessor sortJobProcessor, IServiceProvider serviceProvider)
        {
            _sortJobProcessor = sortJobProcessor;
            Services = serviceProvider;

            Queue = new Dictionary<Guid, SortJob>();

        }
        public IServiceProvider Services { get; }


        public Dictionary<Guid, SortJob> Queue { get; set; }


        private Task Process()
        {



            while (_runBackGroundProcess)
            {
                Dictionary<Guid, SortJob>? queue = new Dictionary<Guid, SortJob>();
                using (IServiceScope? scope = Services.CreateScope())
                {
                    IBackGroundProcessor? scopedProcessingService =
                        scope.ServiceProvider
                            .GetRequiredService<IBackGroundProcessor>();

                    //await scopedProcessingService.DoWork(stoppingToken);
                    queue = scopedProcessingService.Queue;
                }
                if (queue.Where(x => x.Value.Status == SortJobStatus.Pending).Any())
                {
                    KeyValuePair<Guid, SortJob> pendingJob = queue.Where(x => x.Value.Status == SortJobStatus.Pending).FirstOrDefault();

                    if (pendingJob.Value is null)
                    {
                        continue;
                    }

                    queue[pendingJob.Key] = _sortJobProcessor.Process(pendingJob.Value).Result;
                }
                else
                {
                    Thread.Sleep(5000);
                }
            }
            return Task.CompletedTask;
        }

        public SortJob? GetJob(Guid guid)
        {
            return Queue.ContainsKey(guid) ? Queue[guid] : null;
        }

        public List<SortJob> GetAllJobs()
        {
            List<SortJob> sortJobList = new List<SortJob>();
            if (Queue != null)
            {
                foreach (KeyValuePair<Guid, SortJob> entry in Queue)
                {
                    sortJobList.Add(entry.Value);
                }
            }
            return sortJobList;
        }

        public SortJob PushInQueue(int[] values)
        {
            SortJob? pendingJob = new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Pending,
                duration: null,
                input: values,
                output: null);

            Queue.Add(pendingJob.Id, pendingJob);
            return pendingJob;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _runBackGroundProcess = true;
            return Process();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _runBackGroundProcess = false;
            return Task.CompletedTask;
        }

    }
}

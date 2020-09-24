using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class BackGroundProcessor : IHostedService
    {
        private readonly ISortJobProcessor _sortJobProcessor;
        private bool _runBackGroundProcess;
        private IServiceProvider _srvices;

        public BackGroundProcessor(ISortJobProcessor sortJobProcessor, IServiceProvider serviceProvider)
        {
            _sortJobProcessor = sortJobProcessor;
            _srvices = serviceProvider;
        }

        private Task Process()
        {
            while (_runBackGroundProcess)
            {
                var queue = new Dictionary<Guid, SortJob>();

                using (IServiceScope? scope = _srvices.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider
                                                    .GetRequiredService<IBackgroundTaskQueue>();

                    queue = scopedProcessingService.Queue;
                }

                if (queue.Where(x => x.Value.Status == SortJobStatus.Pending).Any())
                {
                    KeyValuePair<Guid, SortJob> pendingJob = queue
                                                .Where(x => x.Value.Status == SortJobStatus.Pending)
                                                .FirstOrDefault();

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

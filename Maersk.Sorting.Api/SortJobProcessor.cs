using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class SortJobProcessor : ISortJobProcessor
    {
        private readonly ILogger<SortJobProcessor> _logger;
        private Dictionary<string, SortJob> sortJobs = new Dictionary<string, SortJob>();

        public SortJobProcessor(ILogger<SortJobProcessor> logger)
        {
            _logger = logger;
        }

        public List<SortJob> GetAllJobs()
        {
            List<SortJob> sortJobList = new List<SortJob>();
            if (sortJobs != null)
            {
                foreach (KeyValuePair<string, SortJob> entry in sortJobs)
                {
                    sortJobList.Add(entry.Value);
                }
            }
            return sortJobList;
        }




        public async Task<SortJob> Process(SortJob job)
        {
            _logger.LogInformation("Processing job with ID '{JobId}'.", job.Id);

            var stopwatch = Stopwatch.StartNew();

            var output = job.Input.OrderBy(n => n).ToArray();
            await Task.Delay(5000); // NOTE: This is just to simulate a more expensive operation

            var duration = stopwatch.Elapsed;

            _logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", job.Id, duration);

            return new SortJob(
                id: job.Id,
                status: SortJobStatus.Completed,
                duration: duration,
                input: job.Input,
                output: output);
        }
    }
}

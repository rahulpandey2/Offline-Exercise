using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        public Dictionary<Guid, SortJob> Queue { get; set; }

        public BackgroundTaskQueue()
        {
            Queue = new Dictionary<Guid, SortJob>();
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

        public void ResetQueue()
        {
            Queue = new Dictionary<Guid, SortJob>();
        }
    }
}

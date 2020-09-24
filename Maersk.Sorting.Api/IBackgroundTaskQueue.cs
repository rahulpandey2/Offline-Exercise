using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Maersk.Sorting.Api
{
    public interface IBackgroundTaskQueue
    {
        Dictionary<Guid, SortJob> Queue { get; set; }

        List<SortJob> GetAllJobs();
        SortJob? GetJob(Guid guid);
        SortJob PushInQueue(int[] values);
        void ResetQueue();
    }
}
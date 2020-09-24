using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public interface IBackGroundProcessor
    {
        Dictionary<Guid, SortJob> Queue { get; set; }

        List<SortJob> GetAllJobs();
        SortJob? GetJob(Guid guid);

        SortJob PushInQueue(int[] values);
    }
}
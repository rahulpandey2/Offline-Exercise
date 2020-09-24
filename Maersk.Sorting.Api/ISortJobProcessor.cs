using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public interface ISortJobProcessor
    {
        Task<SortJob> Process(SortJob job);

        //Task<List<SortJob>> GetAllJobs();

        //Task<SortJob?> GetOneJob(Guid guid);
    }
}
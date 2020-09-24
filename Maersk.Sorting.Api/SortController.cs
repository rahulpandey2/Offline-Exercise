using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Controllers
{
    [ApiController]
    [Route("sort")]
    public class SortController : ControllerBase
    {
        private readonly ISortJobProcessor _sortJobProcessor;
        private readonly IBackGroundProcessor _backGroundProcessor;

        public SortController(ISortJobProcessor sortJobProcessor, IBackGroundProcessor backGroundProcessor)
        {
            _sortJobProcessor = sortJobProcessor;
            _backGroundProcessor = backGroundProcessor;
        }

        [HttpPost("run")]
        [Obsolete("This executes the sort job asynchronously. Use the asynchronous 'EnqueueJob' instead.")]
        public async Task<ActionResult<SortJob>> EnqueueAndRunJob(int[] values)
        {
            var pendingJob = new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Pending,
                duration: null,
                input: values,
                output: null);

            var completedJob = await _sortJobProcessor.Process(pendingJob);

            return Ok(completedJob);
        }

        [HttpPost]
        public ActionResult<SortJob> EnqueueJob(int[] values)
        {
            return _backGroundProcessor.PushInQueue(values);
        }

        [HttpGet]
        public ActionResult<List<SortJob>> GetJobs()
        {
            return _backGroundProcessor.GetAllJobs();
        }

        [HttpGet("{jobId}")]
        public ActionResult<SortJob?> GetJob(Guid jobId)
        {
            return _backGroundProcessor.GetJob(jobId);
        }
    }
}

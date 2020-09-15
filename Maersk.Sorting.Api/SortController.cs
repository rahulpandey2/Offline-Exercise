using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Controllers
{
    [ApiController]
    [Route("sort")]
    public class SortController : ControllerBase
    {
        private readonly ISortJobProcessor _sortJobProcessor;

        public SortController(ISortJobProcessor sortJobProcessor)
        {
            _sortJobProcessor = sortJobProcessor;
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
        public Task<ActionResult<SortJob>> EnqueueJob(int[] values)
        {
            // TODO: Should enqueue a job to be processed in the background.
            throw new NotImplementedException();
        }

        [HttpGet]
        public Task<ActionResult<SortJob[]>> GetJobs()
        {
            // TODO: Should return all jobs that have been enqueued (both pending and completed).
            throw new NotImplementedException();
        }

        [HttpGet("{jobId}")]
        public Task<ActionResult<SortJob>> GetJob(Guid jobId)
        {
            // TODO: Should return a specific job by ID.
            throw new NotImplementedException();
        }
    }
}

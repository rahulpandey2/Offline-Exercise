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
        private readonly IBackgroundTaskQueue _backGroundTaskQueue;

        public SortController(ISortJobProcessor sortJobProcessor, IBackgroundTaskQueue backGroundtaskQueue)
        {
            _sortJobProcessor = sortJobProcessor;
            _backGroundTaskQueue = backGroundtaskQueue;
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
            return _backGroundTaskQueue.PushInQueue(values);
        }

        [HttpGet]
        public ActionResult<List<SortJob>> GetJobs()
        {
            return _backGroundTaskQueue.GetAllJobs();
        }

        [HttpGet("{jobId}")]
        public ActionResult<SortJob?> GetJob(Guid jobId)
        {
            return _backGroundTaskQueue.GetJob(jobId);
        }

        [HttpGet]
        [Route("ResetQueue")]
        public ActionResult ResetQueue()
        {
            _backGroundTaskQueue.ResetQueue();
            return Ok("Queue cleared successfully");
        }
    }
}

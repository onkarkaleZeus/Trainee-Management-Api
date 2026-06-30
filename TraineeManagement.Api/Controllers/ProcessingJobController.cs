using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs.ProcessingJobDto;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Controllers
{
    [Route("api/processing-jobs")]
    [ApiController]
    [Authorize]
    public class ProcessingJobController(
        IProcessingJobService service
    ) : ControllerBase
    {
        private readonly IProcessingJobService _service = service;

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProcessingJobResponse>> GetProcessingJobById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero");

            var respone = await _service.GetProcessingJobById(id);

            return Ok(respone);
        }

        [HttpPost("{id:int}/retry")]
        public async Task<ActionResult<ProcessingJobResponse>> RetryProcessingJob(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero");

            var respone = await _service.RetryProcessingJob(id);

            return Ok(respone);
        }
    }
}
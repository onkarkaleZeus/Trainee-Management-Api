using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.DTOs.LearningTaskDto;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Controllers
{
    [Route("api/learning-tasks")]
    [ApiController]
    [Authorize]
    public class LearningTaskController(
        ILearningTaskService service
    ) : ControllerBase
    {
        private readonly ILearningTaskService _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LearningTaskResponse>>> GetAllLearningTasks(
            int? pageNumber,
            int? pageSize,
            string? search,
            LearningTaskStatus? status
        )
        {

                var (totalRecords, response) = await _service.GetAllLearningTasks(pageNumber, pageSize, search, status);

                return Ok(new PaginationTaskResponse
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 10,
                    TotalRecords = totalRecords,
                    Data = response
                });

        }

        // get Trainee Details using id parameter
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LearningTaskResponse>> GetLearningTaskById(int id)
        {

                if (id <= 0)
                    return BadRequest("Id must be greater than zero");

                var response = await _service.GetLearningTaskById(id);

                return Ok(response);
        }


        // Add a new Trainee
        [HttpPost]
        public async Task<ActionResult<LearningTaskResponse>> AddLearningTask([FromBody] CreateLearningTaskRequest request)
        {

                var response = await _service.AddLearningTask(request);

                return Ok(response);

        }

        // Update Trainee Details
        [HttpPut("{id:int}")]
        public async Task<ActionResult<LearningTaskResponse>> UpdateLearningTaskById(int id, [FromBody] UpdateLearningTaskRequest request)
        {
                if (id <= 0)
                    return BadRequest("Id must be greater than zero");

                var response = await _service.UpdateLearningTaskById(id, request);

                return Ok(response);

        }

        // Delete Api for Deleting
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteLearningById(int id)
        {
                if (id <= 0)
                    return BadRequest("Id must be greater than zero");

                await _service.DeleteLearningTaskById(id);

                return NoContent();
        }
    }
}
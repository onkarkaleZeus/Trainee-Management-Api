using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs.TaskAssignmentDto;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Controllers
{
    [Route("api/task-assignments")]
    [ApiController]
    [Authorize]
    public class TaskAssignmentController(
        ITaskAssignmentService service
    ) : ControllerBase
    {
        private readonly ITaskAssignmentService _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskAssignmentResponse>>> GetAllTasks(
        // int? pageNumber,
        // int? pageSize,
        // string? search,
        // MentorStatus? status
        )
        {

            var response = await _service.GetAllTasks();

            return Ok(response);

        }

        // get Trainee Details using id parameter
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskAssignmentResponse>> GetTaskAssignmentById(int id)
        {

            if (id <= 0)
                return BadRequest("Id must be greater than zero");

            var response = await _service.GetTaskAssignmentById(id);

            return Ok(response);
        }


        // Add a new Trainee
        [HttpPost]
        public async Task<ActionResult<TaskAssignmentResponse>> AddTaskAssignment([FromBody] CreateTaskAssignmentRequest request)
        {
            var response = await _service.AddTaskAssignment(request);

            return Ok(response);
        }

        // Update Trainee Details
        [HttpPut("{id:int}/status")]
        public async Task<ActionResult<bool>> UpdateTaskAssignmentById(int id, [FromBody] UpdateTaskAssignmentRequest request)
        {

            if (id <= 0)
                return BadRequest("Id must be greater than zero");

            var response = await _service.UpdateTaskAssignmentById(id, request);

            return Ok(new
            {
                response,
                message = "Task Status changed successfully"
            });
        }
    }
}
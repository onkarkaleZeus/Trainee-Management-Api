using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs.TraineeDto;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class TraineesController(ITraineeService service) : ControllerBase
    {

        private readonly ITraineeService _service = service;

        // get all the Trainees details
        [HttpGet]
        public async Task<ActionResult<PaginationTraineeResponse>> GetAllTrainees(
            int? pageNumber,
            int? pageSize,
            string? search,
            TraineeStatus? status
        )
        {
            var (totalrecords, response) = await _service.GetAllTrainees(pageNumber, pageSize, search, status);

            return Ok(new PaginationTraineeResponse()
            {
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? 10,
                TotalRecords = totalrecords,
                Data = response ?? []
            });
        }

        // get Trainee Details using id parameter
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetTraineeById(int id)
        {

            if (id <= 0)
                return BadRequest("Id must be greater than zero");

            var response = await _service.GetTraineeById(id);

            return Ok(response);
        }


        // Add a new Trainee
        [HttpPost]
        public async Task<ActionResult> AddNewTrainee([FromBody] CreateTraineeRequest request)
        {

            var response = await _service.AddTrainee(request);

            return Ok(response);
        }

        // Update Trainee Details
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateTraineeById(int id, [FromBody] UpdateTraineeRequest request)
        {

            if (id <= 0)
                return BadRequest("Id must be greater than zero");

            var response = await _service.UpdateTraineeById(id, request);

            return Ok(response);

        }

        // Delete Api for Deleting
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteTraineeById(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than zero");

            await _service.DeleteTraineeById(id);

            return NoContent();
        }
    }
}
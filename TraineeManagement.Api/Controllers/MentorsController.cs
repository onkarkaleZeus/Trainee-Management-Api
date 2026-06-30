using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TraineeManagement.Api.DTOs.MentorDto;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class MentorsController(
        IMentorService service
    ) : ControllerBase
    {

        private readonly IMentorService _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MentorResponse>>> GetAllMentors(
            int? pageNumber,
            int? pageSize,
            string? search,
            MentorStatus? status
        )
        {

            var (totalRecords, response) = await _service.GetAllMentors(pageNumber, pageSize, search, status);

            return Ok(new PaginationMentorResponse
            {
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? 1,
                TotalRecords = totalRecords,
                Data = response
            });
        }

        // get Trainee Details using id parameter
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MentorResponse>> GetMentorById(int id)
        {
            if (id <= 0)
                return BadRequest("Id must be greater than zero");

            var response = await _service.GetMentorById(id) ;

            return Ok(response);
        }


        // Add a new Trainee
        [HttpPost]
        public async Task<ActionResult<MentorResponse>> AddNewMentor([FromBody] CreateMentorRequest request)
        {
            var response = await _service.AddMentor(request);

            return Ok(response);
        }

        // Update Trainee Details
        [HttpPut("{id:int}")]
        public async Task<ActionResult<MentorResponse>> UpdateMentorById(int id, [FromBody] UpdateMentorRequest request)
        {

            if (id <= 0)
                return BadRequest("Id must be greater than zero");

            var response = await _service.UpdateMentorById(id, request);
            
            return Ok(response);

        }

        // Delete Api for Deleting
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteTraineeById(int id)
        {

            if (id <= 0)
                return BadRequest("Id must be greater than zero");

            await _service.DeleteMentorById(id);

            return NoContent();
        }
    }
}
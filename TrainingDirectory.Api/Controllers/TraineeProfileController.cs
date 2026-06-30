// using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using TrainingDirectory.Api.DTOs.TraineeProfileDto;


namespace TrainingDirectory.Api.Controllers
{   
    [Route("api/[Controller]")]
    [ApiController]
    public class TraineeProfileController : ControllerBase
    {
        [HttpGet("{id:int}")]
        public ActionResult<TraineeProfileResponse> GetTraineeProfile(int id)
        {
            return Ok(new TraineeProfileResponse
            {
                Id = id,
                Name = "Onkar Kale",
                Email = "onkarkale@gmail.com",
                Designation = "SE"
            });
        }
    }
}
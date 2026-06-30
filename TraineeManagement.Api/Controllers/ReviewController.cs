using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs.ReviewDto;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    [Authorize]
    public class ReviewController(
        IReviewService service
    ) : ControllerBase
    {
        private readonly IReviewService _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetAllReviews(
        // int? pageNumber,
        // int? pageSize,
        // string? search,
        // ReviewStatus? status
        )
        {

            var response = await _service.GetAllReviews();

            return Ok(response);

        }

        // get Trainee Details using id parameter
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReviewResponse>> GetReviewById(int id)
        {

            if (id <= 0)
                return BadRequest("Id must be greater than zero");

            var response = await _service.GetReviewById(id);

            return Ok(response);

        }


        // Add a new Trainee
        [HttpPost]
        public async Task<ActionResult<ReviewResponse>> AddSubmission([FromBody] CreateReviewRequest request)
        {

            var response = await _service.AddReview(request);

            return Ok(response);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs.SubmissionDto;
using TraineeManagement.Api.DTOs.SubmissionFilesDto;
using TraineeManagement.Api.Interfaces;


namespace TraineeManagement.Api.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    [Authorize]
    public class SubmissionController(
        ISubmissionService submissionService,
        ISubmissionFileService submissionFileService
    ) : ControllerBase
    {
        private readonly ISubmissionService _submissionService = submissionService;
        private readonly ISubmissionFileService _submissionFileService = submissionFileService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubmissionResponse>>> GetAllSubmissions(
        )
        {
            var response = await _submissionService.GetAllSubmissions();

            return Ok(response);

        }

        // get Submission Details using id parameter
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SubmissionResponse>> GetSubmissionById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero");

            var response = await _submissionService.GetSubmissionById(id);

            return Ok(response);
        }


        // Add a new Submission
        [HttpPost]
        public async Task<ActionResult<SubmissionResponse>> AddSubmission([FromBody] CreateSubmissionRequest request)
        {
            var response = await _submissionService.AddSubmission(request);

            return Ok(response);
        }

        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = 20971520)]
        [HttpPost("{submissionId:int}/files")]
        public async Task<ActionResult<SubmissionFileResponse>> AddNewSubmissionFile(int submissionId, IFormFile formFile, int userId)
        {
            var response = await _submissionFileService.UploadFile(submissionId, formFile, userId);

            return StatusCode(202, response);
        }

    }
}
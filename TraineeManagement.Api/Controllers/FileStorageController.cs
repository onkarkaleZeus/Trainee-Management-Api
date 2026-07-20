using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs.SubmissionFilesDto;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Controllers
{   

    [Route("api/submission-files")]
    [ApiController]
    [Authorize]
    public class FileStorageController(
        ISubmissionFileService service
    ) : ControllerBase
    {

        private readonly ISubmissionFileService _service = service ;

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SubmissionFileResponse>> GetSubmissionFileById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero");
            
            var response = await _service.GetSubmissionFileById(id);

            return Ok(response);
        }

        [HttpGet("{id:int}/download")]
        public async Task<ActionResult<FileDownloadResponse>> DownloadFile(int id)
        {

            var file = await _service.DownloadFile(id, User);

            return File(
                file.Stream,
                file.ContentType,
                file.FileName
            ) ;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteFile(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero");

            await _service.DeleteFile(id) ;

            return NoContent();
        }
    }
}
using System.Security.Claims;
using TraineeManagement.Api.DTOs.SubmissionFilesDto;

namespace TraineeManagement.Api.Interfaces
{
    public interface ISubmissionFileService
    {
        Task<SubmissionFileResponse> GetSubmissionFileById(int id);

        Task<SubmissionFileResponse> UploadFile(int submissionId, IFormFile formFile, int userId);

        Task<FileDownloadResponse> DownloadFile(int id, ClaimsPrincipal User);

        Task DeleteFile(int id);
    }
}
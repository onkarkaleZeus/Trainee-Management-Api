using TraineeManagement.Api.DTOs.SubmissionDto;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface ISubmissionService
    {
        Task<IEnumerable<SubmissionResponse>> GetAllSubmissions(
        // int? pageNumber,
        // int? pageSize,
        // string? search,
        // SubmissionStatus? status
        );
        Task<SubmissionResponse?> GetSubmissionById(int id);
        Task<SubmissionResponse> AddSubmission(CreateSubmissionRequest request);
        // Task<FileStorageResponse> UploadSubmission( List<IFormFile> files );
    }
}
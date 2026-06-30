using SubmissionProcessor.Worker.DTOs.TraineeProfileDto;

namespace SubmissionProcessor.Worker.Interfaces
{
    public interface ITrainingDirectoryService
    {
        Task<TraineeProfileResponse> GetTraineeProfileWithFallbackAsync(int traineeId, CancellationToken cancellationToken);
    }
}
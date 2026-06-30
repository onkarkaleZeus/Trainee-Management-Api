using SubmissionProcessor.Worker.DTOs.TraineeProfileDto;

namespace SubmissionProcessor.Worker.Interfaces
{
    public interface ITrainingDirectoryClient
    {
        Task<TraineeProfileResponse?> GetProfiles(int id, string? correlationId, CancellationToken cancellationToken);
    }
}
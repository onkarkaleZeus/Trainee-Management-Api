using TraineeManagement.Api.DTOs.ProcessingJobDto;

namespace TraineeManagement.Api.Interfaces
{
    public interface IProcessingJobService
    {
        Task<ProcessingJobResponse> GetProcessingJobById(int id);
        Task<ProcessingJobResponse> RetryProcessingJob(int id); 
    }
}
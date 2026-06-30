using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface ISubmissionProcessingPublisherService
    {
        Task PublishAsync(SubmissionProcessingRequested message);
    }
}
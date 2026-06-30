using System.Runtime.CompilerServices;
using TraineeManagement.Api.DTOs.LearningTaskDto;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface ILearningTaskService
    {
        Task<(int, IEnumerable<LearningTaskResponse>)> GetAllLearningTasks(
            int? pageNumber,
            int? pageSize,
            string? search,
            LearningTaskStatus? status
        );
        Task<LearningTaskResponse?> GetLearningTaskById(int id);
        Task<LearningTaskResponse> AddLearningTask(CreateLearningTaskRequest request);
        Task<LearningTaskResponse?> UpdateLearningTaskById(int id, UpdateLearningTaskRequest request);
        Task DeleteLearningTaskById(int id);
    }
}
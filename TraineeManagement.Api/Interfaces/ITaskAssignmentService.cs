using TraineeManagement.Api.DTOs.TaskAssignmentDto;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface ITaskAssignmentService
    {
        Task<IEnumerable<TaskAssignmentResponse>> GetAllTasks(
        // int? pageNumber,
        // int? pageSize,
        // string? search,
        // TaskAssignmentStatus? status
        );
        Task<TaskAssignmentResponse?> GetTaskAssignmentById(int id);
        Task<TaskAssignmentResponse> AddTaskAssignment(CreateTaskAssignmentRequest request);
        Task<bool> UpdateTaskAssignmentById(int id, UpdateTaskAssignmentRequest request);
    }
}
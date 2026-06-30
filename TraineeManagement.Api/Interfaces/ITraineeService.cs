using TraineeManagement.Api.DTOs.TraineeDto;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface ITraineeService
    {
        Task<(int, IEnumerable<TraineeResponse>?)> GetAllTrainees(
            int? pageNumber,
            int? pageSize,
            string? search,
            TraineeStatus? status
        );
        Task<TraineeResponse?> GetTraineeById(int id);
        Task<TraineeResponse> AddTrainee(CreateTraineeRequest request);
        Task<TraineeResponse?> UpdateTraineeById(int id, UpdateTraineeRequest request);
        Task DeleteTraineeById(int id);
    }
}
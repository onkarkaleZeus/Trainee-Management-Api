using TraineeManagement.Api.DTOs.MentorDto;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface IMentorService
    {
        Task<(int, IEnumerable<MentorResponse>)> GetAllMentors(
            int? pageNumber,
            int? pageSize,
            string? search,
            MentorStatus? status
        );
        Task<MentorResponse?> GetMentorById(int id);
        Task<MentorResponse> AddMentor(CreateMentorRequest request);
        Task<MentorResponse?> UpdateMentorById(int id, UpdateMentorRequest request);
        Task DeleteMentorById(int id);

    }
}
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs.MentorDto;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventSource;

namespace TraineeManagement.Api.Services
{
    public class MentorServices(
        AppDbContext context,
        ILogger<MentorServices> logger
    ) : IMentorService
    {
        private readonly AppDbContext _context = context;
        private readonly ILogger<MentorServices> _logger = logger;

        public async Task<(int, IEnumerable<MentorResponse>)> GetAllMentors(
            int? pageNumber,
            int? pageSize,
            string? search,
            MentorStatus? status
        )
        {
            var query = await _context.Mentors.ToListAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = _context.Mentors
                    .Where(x =>
                            x.FirstName.ToLower().Contains(search) ||
                            x.LastName.ToLower().Contains(search) ||
                            x.Email.ToLower().Contains(search) ||
                            x.Expertise.ToLower().Contains(search)).ToList();
            }

            if (status != null)
            {
                query = query
                    .Where(
                        x => x.Status == status
                    ).ToList();
            }

            if (query.Count == 0 && (!string.IsNullOrWhiteSpace(search) || status != null))
            {
                throw new KeyNotFoundException("Mentor records not found");
            }

            _logger.LogInformation("Mentor records found: {queryCount}", query.Count);
            return (
                query.Count,
                query
                    .Skip(((pageNumber ?? 1) - 1) * (pageSize ?? 10))
                    .Take(pageSize ?? 10)
                    .Select(MapDtoToMentorResponse)
            );
        }

        public async Task<MentorResponse?> GetMentorById(int id)
        {

            var FoundMentor = await _context.Mentors.FindAsync(id) ??
                throw new Exception($"Mentor not found for Id: {id}");

            _logger.LogInformation("Mentor with Id {id} found: {FoundMentor.FirstName} {FoundMentor.LastName}", id, FoundMentor.FirstName, FoundMentor.LastName);
            return MapDtoToMentorResponse(FoundMentor);

        }
        public async Task<MentorResponse> AddMentor(CreateMentorRequest request)
        {

            Mentor newMentor = new Mentor
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Expertise = request.Expertise,
                Status = request.Status
            };

            await _context.Mentors.AddAsync(newMentor);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New Mentor added with Id {id}", newMentor.Id);
            return MapDtoToMentorResponse(newMentor);

        }

        public async Task<MentorResponse?> UpdateMentorById(int id, UpdateMentorRequest request)
        {

            var mentor = await _context.Mentors.FindAsync(id) ??
                throw new Exception($"Mentor with Id {id} not found");

            mentor.FirstName = request.FirstName;
            mentor.LastName = request.LastName;
            mentor.Email = request.Email;
            mentor.Expertise = request.Expertise;
            mentor.Status = request.Status;
            mentor.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Mentor with Id {id} updated successfully", id);
            return MapDtoToMentorResponse(mentor);
        }

        public async Task DeleteMentorById(int id)
        {

            var mentor = await _context.Mentors.FindAsync(id) ??
                throw new Exception($"Mentor with Id {id} not found");

            _context.Mentors.Remove(mentor);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Mentor with Id {id} deleted successfully.", id);

        }

        public MentorResponse MapDtoToMentorResponse(Mentor mentor)
        {
            return new MentorResponse
            {
                Id = mentor.Id,
                FirstName = mentor.FirstName,
                LastName = mentor.LastName,
                Email = mentor.Email,
                Expertise = mentor.Expertise,
                Status = mentor.Status,
                CreatedDate = mentor.CreatedDate,
                UpdatedDate = mentor.UpdatedDate
            };
        }
    }
}
using TraineeManagement.Api.Models;
using TraineeManagement.Api.DTOs.TraineeDto;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventSource;
using Microsoft.Extensions.Caching.Distributed;

namespace TraineeManagement.Api.Services
{
    public class TraineeServices(AppDbContext context, ILogger<TraineeServices> logger, ICacheService cache) : ITraineeService
    {

        private readonly AppDbContext _context = context;
        private readonly ILogger<TraineeServices> _logger = logger;
        private readonly ICacheService _cache = cache;

        public async Task<(int, IEnumerable<TraineeResponse>?)> GetAllTrainees(
            int? pageNumber,
            int? pageSize,
            string? search,
            TraineeStatus? status
        )
        {

            var query = await _context.Trainees.ToListAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = _context.Trainees
                    .Where(x =>
                            x.FirstName.ToLower().Contains(search) ||
                            x.LastName.ToLower().Contains(search) ||
                            x.Email.ToLower().Contains(search) ||
                            x.TechStack.ToLower().Contains(search)).ToList();
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
                throw new KeyNotFoundException("Trainee records not found");
            }

            _logger.LogInformation("Trainee records found: {queryCount}", query.Count);
            return (
                query.Count,
                query
                    .Skip(((pageNumber ?? 1) - 1) * (pageSize ?? 10))
                    .Take(pageSize ?? 10)
                    .Select(MapToTraineeResponseDTO)
            );
        }

        public async Task<TraineeResponse?> GetTraineeById(int id)
        {
            var hit = await _cache.GetAsync<TraineeResponse>($"trainee:{id}");
            if (hit != null)
            {
                return hit;
            }

            var FoundTrainee = await _context.Trainees.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id) ??
                 throw new KeyNotFoundException($"Trainee not found for Id: {id}");

            var response = MapToTraineeResponseDTO(FoundTrainee);
            await _cache.SetAsync<TraineeResponse>($"trainee:{id}", response);

            _logger.LogInformation("Trainee with Id {id} found: {responseFirstName} {responseLastName}", id, FoundTrainee.FirstName, FoundTrainee.LastName);
            return response;

        }
        public async Task<TraineeResponse> AddTrainee(CreateTraineeRequest request)
        {

            Trainee newTrainee = new Trainee();
            newTrainee.FirstName = request.FirstName;
            newTrainee.LastName = request.LastName;
            newTrainee.Email = request.Email;
            newTrainee.TechStack = request.TechStack;
            newTrainee.Status = request.Status;

            await _context.Trainees.AddAsync(newTrainee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New Trainee added.");
            return MapToTraineeResponseDTO(newTrainee);
        }

        public async Task<TraineeResponse?> UpdateTraineeById(int id, UpdateTraineeRequest request)
        {

            await _cache.DeleteAsync($"trainee:{id}");

            var trainee = await _context.Trainees.FindAsync(id) ??
                throw new KeyNotFoundException($"Trainee with Id {id} not found");

            trainee.FirstName = request.FirstName;
            trainee.LastName = request.LastName;
            trainee.Email = request.Email;
            trainee.Status = request.Status;
            trainee.TechStack = request.TechStack;
            trainee.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Trainee with Id {id} updated successfully", id);
            return MapToTraineeResponseDTO(trainee);

        }

        public async Task DeleteTraineeById(int id)
        {

            await _cache.DeleteAsync($"trainee:{id}");

            var trainee = await _context.Trainees.FindAsync(id) ??
                throw new KeyNotFoundException($"Trainee with Id {id} not found");

            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Trainee with Id {id} deleted successfully.", id);

        }

        // model to TraineeResponse Mapping
        public TraineeResponse MapToTraineeResponseDTO(Trainee trainee)
        {
            return new TraineeResponse
            {
                Id = trainee.Id,
                FirstName = trainee.FirstName,
                LastName = trainee.LastName,
                Email = trainee.Email,
                TechStack = trainee.TechStack,
                Status = trainee.Status,
                CreatedDate = trainee.CreatedDate,
                UpdatedDate = trainee.UpdatedDate
            };
        }
    }
}
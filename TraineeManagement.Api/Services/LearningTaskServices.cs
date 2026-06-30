using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs.LearningTaskDto;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace TraineeManagement.Api.Services
{
    public class LearningTaskServices(
        AppDbContext context,
        ILogger<LearningTaskServices> logger
    ) : ILearningTaskService
    {
        private readonly AppDbContext _context = context;
        private readonly ILogger<LearningTaskServices> _logger = logger;

        public async Task<(int, IEnumerable<LearningTaskResponse>)> GetAllLearningTasks(
            int? pageNumber,
            int? pageSize,
            string? search,
            LearningTaskStatus? status
        )
        {
            var query = await _context.LearningTasks.ToListAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = _context.LearningTasks
                    .Where(x =>
                            x.Title.ToLower().Contains(search) ||
                            x.Description.ToLower().Contains(search) ||
                            x.ExpectedTechStack.ToLower().Contains(search)).ToList();
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
                _logger.LogWarning("No Learning Task records found");
                return (0, null);
            }

            _logger.LogInformation("Learning Task records found: {queryCount}", query.Count);
            return (
                query.Count,
                query
                    .Skip(((pageNumber ?? 1) - 1) * (pageSize ?? 10))
                    .Take(pageSize ?? 10)
                    .Select(MapDtoToLearningTaskResponse)
            );
        }

        public async Task<LearningTaskResponse?> GetLearningTaskById(int id)
        {
            var learningTask = await _context.LearningTasks.FindAsync(id) ??
                throw new KeyNotFoundException("Learning Task not found");

            _logger.LogInformation("Learning Task with Id {id} found: {learningTaskTitle}", id, learningTask.Title);
            return MapDtoToLearningTaskResponse(learningTask);
        }
        public async Task<LearningTaskResponse> AddLearningTask(CreateLearningTaskRequest request)
        {

            LearningTask newLearningTask = new LearningTask
            {
                Title = request.Title,
                Description = request.Description,
                ExpectedTechStack = request.ExpectedTechStack,
                DueDate = request.DueDate,
                Status = request.Status
            };

            await _context.LearningTasks.AddAsync(newLearningTask);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New Learning Task added with Id {id}", newLearningTask.Id);
            return MapDtoToLearningTaskResponse(newLearningTask);

        }

        public async Task<LearningTaskResponse?> UpdateLearningTaskById(int id, UpdateLearningTaskRequest request)
        {
            var learningTask = await _context.LearningTasks.FindAsync(id) ??
                throw new KeyNotFoundException($"Learning Task with Id {id} not found");

            learningTask.Title = request.Title;
            learningTask.Description = request.Description;
            learningTask.ExpectedTechStack = request.ExpectedTechStack;
            learningTask.DueDate = request.DueDate;
            learningTask.Status = request.Status;
            learningTask.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Learning Task with Id {id} updated successfully", id);
            return MapDtoToLearningTaskResponse(learningTask);
        }

        public async Task DeleteLearningTaskById(int id)
        {

            var learningTask = await _context.LearningTasks.FindAsync(id) ??
                throw new KeyNotFoundException($"Learning with Id {id} not found");

            _context.LearningTasks.Remove(learningTask);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Learing Task with Id {id} deleted successfully.", id);

        }

        public LearningTaskResponse MapDtoToLearningTaskResponse(LearningTask learningTask)
        {
            return new LearningTaskResponse
            {
                Id = learningTask.Id,
                Title = learningTask.Title,
                Description = learningTask.Description,
                ExpectedTechStack = learningTask.ExpectedTechStack,
                DueDate = learningTask.DueDate,
                Status = learningTask.Status,
                CreatedDate = learningTask.CreatedDate,
                UpdatedDate = learningTask.UpdatedDate
            };
        }
    }
}
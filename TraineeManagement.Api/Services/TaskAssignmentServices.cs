using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs.TaskAssignmentDto;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventSource;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace TraineeManagement.Api.Services
{
    public class TaskAssignmentServices(
        AppDbContext context,
        ILogger<TaskAssignmentServices> logger,
        ICacheService cache

    ) : ITaskAssignmentService
    {
        private readonly AppDbContext _context = context;
        private readonly ILogger<TaskAssignmentServices> _logger = logger;
        private readonly ICacheService _cache = cache;

        public async Task<IEnumerable<TaskAssignmentResponse>> GetAllTasks(
        // int? pageNumber,
        // int? pageSize,
        // string? search,
        // TaskAssignmentStatus? status
        )
        {
            var records = await _context.TaskAssignments
                                        .Include(t => t.Trainee)
                                        .Include(m => m.Mentor)
                                        .Include(lt => lt.LearningTask)
                                        .ToListAsync();

            _logger.LogInformation("Task assignment records retrieved: {recordCount}", records.Count);
            return records.Select(MapDtoToTaskAssignmentResponse);

        }

        public async Task<TaskAssignmentResponse?> GetTaskAssignmentById(int id)
        {

            var hit = await _cache.GetAsync<TaskAssignmentResponse>($"task-assignment:{id}");
            if (hit != null)
            {
                return hit;
            }

            var taskAssignment = await _context.TaskAssignments
                                        .Include(t => t.Trainee)
                                        .Include(m => m.Mentor)
                                        .Include(lt => lt.LearningTask)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(t => t.Id == id);
            if (taskAssignment == null)
            {
                throw new KeyNotFoundException($"Task not found for Id: {id}");
            }

            var response = MapDtoToTaskAssignmentResponse(taskAssignment);
            await _cache.SetAsync<TaskAssignmentResponse>($"task-assignment:{id}", response);

            _logger.LogInformation("Task Assignment with Id {id} found", id);
            return response;

        }

        public async Task<TaskAssignmentResponse> AddTaskAssignment(CreateTaskAssignmentRequest request)
        {

            var trainee = await _context.Trainees.FindAsync(request.TraineeId) ??
                throw new Exception("Trainee not found");

            var mentor = await _context.Mentors.FindAsync(request.MentorId) ??
                throw new Exception("Mentor not found");

            var learningTask = await _context.LearningTasks.FindAsync(request.LearningTaskId) ??
                throw new Exception("Learning Task not found");

            if (request.DueDate < request.AssignedDate)
            {
                throw new ArgumentException("Due date cannot be before Assigned date");
            }

            TaskAssignment newTaskAssignment = new TaskAssignment()
            {
                TraineeId = request.TraineeId,
                MentorId = request.MentorId,
                LearningTaskId = request.LearningTaskId,
                AssignedDate = request.AssignedDate,
                DueDate = request.DueDate,
                Status = request.Status,
                Remarks = request.Remarks ?? ""
            };

            await _context.TaskAssignments.AddAsync(newTaskAssignment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New Task added with Id {id}", newTaskAssignment.Id);
            return MapDtoToTaskAssignmentResponse(newTaskAssignment);
        }

        public async Task<bool> UpdateTaskAssignmentById(int id, UpdateTaskAssignmentRequest request)
        {
            await _cache.DeleteAsync($"task-assignment:{id}");

            var taskAssignment = await _context.TaskAssignments
                                        .FirstOrDefaultAsync(t => t.Id == id);
            if (taskAssignment == null)
            {
                throw new KeyNotFoundException($"Task with Id {id} not found");
            }

            taskAssignment.Status = request.Status;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Task assignment with Id {id} updated successfully", id);
            return true;
        }

        public TaskAssignmentResponse MapDtoToTaskAssignmentResponse(TaskAssignment taskAssignment)
        {
            return new TaskAssignmentResponse()
            {
                Id = taskAssignment.Id,
                TraineeId = taskAssignment.TraineeId,
                TraineeName = $"{taskAssignment.Trainee.FirstName} {taskAssignment.Trainee.LastName}",
                MentorId = taskAssignment.MentorId,
                MentorName = $"{taskAssignment.Mentor.FirstName} {taskAssignment.Mentor.LastName}",
                LearningTaskId = taskAssignment.LearningTaskId,
                TaskTitle = taskAssignment.LearningTask.Title,
                TaskDescription = taskAssignment.LearningTask.Description,
                AssignedDate = taskAssignment.AssignedDate,
                DueDate = taskAssignment.DueDate,
                Status = taskAssignment.Status,
                Remarks = taskAssignment.Remarks,
                CreatedDate = taskAssignment.CreatedAt,
                UpdatedDate = taskAssignment.UpdatedAt
            };
        }
    }
}
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs.SubmissionDto;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventSource;
using Microsoft.VisualBasic;

namespace TraineeManagement.Api.Services
{
    public class SubmissionServices(
        ILogger<SubmissionServices> logger,
        ICacheService cache,
        AppDbContext context

    ) : ISubmissionService
    {
        private readonly AppDbContext _context = context;
        private readonly ILogger<SubmissionServices> _logger = logger;
        private readonly ICacheService _cache = cache;

        public async Task<IEnumerable<SubmissionResponse>> GetAllSubmissions(
        // int? pageNumber,
        // int? pageSize,
        // string? search,
        // TaskAssignmentStatus? status
        )
        {
            var records = await _context.Submissions
                                        .Include(x => x.TaskAssignment)
                                        .ToListAsync();

            _logger.LogInformation("Submission records retrieved : {recordCount}", records.Count);
            return records.Select(MapDtoToSubmissionResponse);

        }

        public async Task<SubmissionResponse?> GetSubmissionById(int id)
        {
            var hit = await _cache.GetAsync<SubmissionResponse>($"submission-summary:{id}");
            if (hit != null)
            {
                return hit;
            }

            var submission = await _context.Submissions
                                        .Include(x => x.TaskAssignment)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(t => t.Id == id);
            if (submission == null)
            {
                throw new KeyNotFoundException($"Submission not found for Id: {id}");
            }

            var response = MapDtoToSubmissionResponse(submission);
            await _cache.SetAsync<SubmissionResponse>($"submission-summary:{id}", response);

            _logger.LogInformation("Submission with Id {id} found", id);
            return response;

        }

        public async Task<SubmissionResponse> AddSubmission(CreateSubmissionRequest request)
        {

            var taskAssignment = await _context.TaskAssignments.FindAsync(request.TaskAssignmentId) ??
                throw new ArgumentException("Task Assignment Id is invalid");

            Submission newSubmission = new Submission()
            {
                TaskAssignmentId = request.TaskAssignmentId,
                SubmissionUrl = request.SubmissionUrl,
                Notes = request.Notes,
                SubmittedDate = request.SubmittedDate,
                Status = request.Status
            };

            await _context.Submissions.AddAsync(newSubmission);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New Submission with Id {id} added", newSubmission.Id);
            return MapDtoToSubmissionResponse(newSubmission);
        }

        public SubmissionResponse MapDtoToSubmissionResponse(Submission submission)
        {
            return new SubmissionResponse()
            {
                Id = submission.Id,
                TaskAssignmentId = submission.TaskAssignment.Id,
                TaskAssignmentDate = submission.TaskAssignment.AssignedDate,
                TaskDueDate = submission.TaskAssignment.DueDate,
                TaskAssignmentStatus = submission.TaskAssignment.Status,
                SubmissionUrl = submission.SubmissionUrl,
                Notes = submission.Notes,
                SubmittedDate = submission.SubmittedDate,
                CreatedDate = submission.CreatedAt,
                UpdatedDate = submission.UpdatedAt
            };
        }

    }
}
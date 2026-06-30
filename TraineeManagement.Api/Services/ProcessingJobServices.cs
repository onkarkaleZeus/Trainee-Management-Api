using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs.ProcessingJobDto;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services
{
    public class ProcessingJobServices : IProcessingJobService
    {
        private readonly AppDbContext _context ;

        private readonly ILogger<ProcessingJobServices> _logger;
        private readonly ISubmissionProcessingPublisherService _publisher ;

        public ProcessingJobServices(
            AppDbContext context,
            ILogger<ProcessingJobServices> logger,
            ISubmissionProcessingPublisherService publisher
        )
        {
            _context = context;
            _logger = logger;
            _publisher = publisher;
        }
        public async Task<ProcessingJobResponse> GetProcessingJobById(int id)
        {
            var processingJob = await _context.ProcessingJobs.FindAsync(id) ??
                throw new KeyNotFoundException($"Processing Job not found with Id {id}");

            _logger.LogInformation("Processing Job found for Id {id}", id);
            return MapToProcessingJobDto(processingJob);
        }

        public async Task<ProcessingJobResponse> RetryProcessingJob(int id)
        {
            var processingJob = await _context.ProcessingJobs.FindAsync(id) ?? 
                throw new KeyNotFoundException($"Processing Job not found with Id {id}");
            
            using var transaction = await _context.Database.BeginTransactionAsync();

            var message = new SubmissionProcessingRequested()
            {
                CorrelationId = processingJob.CorrelationId,
                SubmissionId = processingJob.SubmissionId,
                FileId = processingJob.FileId
            };

            try
            {
                processingJob.Status = ProcessingJobStatus.Queued;
                await _context.SaveChangesAsync();

                await _publisher.PublishAsync(message);
                await transaction.CommitAsync();

            }
             catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear(); 

                throw new Exception($"Connection to rabbit failed during retry: {ex.Message}");
            }
            
            return MapToProcessingJobDto(processingJob);
        }

        public ProcessingJobResponse MapToProcessingJobDto( ProcessingJob processingJob )
        {
            return new ProcessingJobResponse
            {
                Id = processingJob.Id,
                MessageId = processingJob.MessageId.ToString(),
                CorrelationId = processingJob.CorrelationId,
                Status = processingJob.Status,
                SubmissionId = processingJob.SubmissionId,
                FileId = processingJob.FileId,
                Attempt = processingJob.Attempt,
                ErrorSummary = processingJob.ErrorSummary ?? "",
                StartedAt = processingJob.StartedAt,
                CompletedAt = processingJob.CompletedAt
            }; 
        }
    }
}
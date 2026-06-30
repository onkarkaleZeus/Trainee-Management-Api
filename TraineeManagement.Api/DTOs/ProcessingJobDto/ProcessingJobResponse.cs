using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.ProcessingJobDto
{
    public class ProcessingJobResponse
    {
        public int Id { get; set; }

        public string MessageId { get; set; } = string.Empty ;

        public string CorrelationId { get; set; } = string.Empty ;
        public ProcessingJobStatus Status { get; set; } = ProcessingJobStatus.Queued ;

        public int SubmissionId { get; set; } 

        public int FileId { get; set; } 

        public int Attempt { get; set; } = 0 ;

        public string? ErrorSummary { get; set; }

        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
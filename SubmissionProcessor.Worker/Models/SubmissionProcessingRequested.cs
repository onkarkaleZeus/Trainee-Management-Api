using Microsoft.Extensions.Options;

namespace SubmissionProcessor.Worker.Models
{
    public class SubmissionProcessingRequested
    {
        public Guid MessageId { get; set; }

        public string? CorrelationId { get; set; }

        // submission id
        public int SubmissionId { get; set; }

        // submission file id
        public int FileId { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.Now;

        public int ContractVersion { get; set; } = 1;
    }
}
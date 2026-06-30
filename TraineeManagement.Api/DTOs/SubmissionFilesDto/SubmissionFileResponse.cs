namespace TraineeManagement.Api.DTOs.SubmissionFilesDto
{
    public class SubmissionFileResponse
    {
        public int Id { get; set; }

        public string TraceId {get; set;} = string.Empty;

        public string OriginalFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
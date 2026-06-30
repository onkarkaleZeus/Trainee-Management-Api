using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.ReviewDto
{
    public class ReviewResponse
    {
        public int Id { get; set; }

        public int SubmissionId { get; set; }
        public string SubmissionUrl { get; set; } = string.Empty ;
        public DateTime SubmissionDate { get; set; } = DateTime.Now ;

        public int MentorId { get; set; }
        public string MentorName { get; set; } = string.Empty ;

        public string Feedback { get; set; } = string.Empty ;

        public int? Score { get; set; } = 0 ;

        public ReviewStatus Status { get; set; } = ReviewStatus.Rejected ;

        public DateTime ReviewedDate { get; set; } = DateTime.Now ;

        public DateTime CreatedDate { get; set; } = DateTime.Now ;
        
        public DateTime UpdatedDate { get; set; } = DateTime.Now ;
    }
}
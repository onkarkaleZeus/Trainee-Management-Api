using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.ReviewDto
{
    public class CreateReviewRequest
    {
        [Required(ErrorMessage = "Submission Id is required")]
        public int SubmissionId { get; set; }


        [Required(ErrorMessage = "Mentor Id is required")]
        public int MentorId { get; set; }

        [Required( ErrorMessage = "Feedback is required" )]
        public string Feedback { get; set; } = string.Empty ;

        public int? Score { get; set; } = 0 ;

        [Required( ErrorMessage = "Review status is required" )]
        public ReviewStatus Status { get; set; } = ReviewStatus.Rejected ;

        [Required(ErrorMessage = "Review date is required")]
        public DateTime ReviewedDate { get; set; } = DateTime.Now ;

    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TraineeManagement.Api.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Submission))]
        public int SubmissionId { get; set; }
        public Submission Submission { get; set; } = null! ;

        [ForeignKey(nameof(Mentor))]
        public int MentorId { get; set; }
        public Mentor Mentor { get; set; } = null! ;

        [Required( ErrorMessage = "Feedback is required" )]
        public string Feedback { get; set; } = string.Empty ;

        public int? Score { get; set; } = 0 ;

        [Required( ErrorMessage = "Review status is required" )]
        public ReviewStatus ReviewStatus { get; set; } = ReviewStatus.Rejected ;

        [Required(ErrorMessage = "Review date is required")]
        public DateTime ReviewedDate { get; set; } = DateTime.Now ;

        public DateTime CreatedAt { get; set; } = DateTime.Now ;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now ;

    }

    public enum ReviewStatus
    {
        Accepted,
        ChangesRequired,
        Rejected
    }
}
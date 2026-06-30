using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace TraineeManagement.Api.Models
{
    public class Submission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(TaskAssignment))]
        public int TaskAssignmentId { get; set; }
        public TaskAssignment TaskAssignment { get; set; } = null! ;

        [Required( ErrorMessage = "Url is required" )]
        [Url]
        public string SubmissionUrl { get; set; } = string.Empty ;

        public string Notes { get; set; } = string.Empty ;

        public DateTime SubmittedDate { get; set; } = DateTime.Now ;

        public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;

        public DateTime CreatedAt { get; set; } = DateTime.Now ;
        public DateTime UpdatedAt { get; set; } = DateTime.Now ;

    }

    public enum SubmissionStatus
    {
        Submitted,
        Resubmitted
        
    }
}
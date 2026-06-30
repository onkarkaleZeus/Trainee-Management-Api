using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.SubmissionDto
{
    public class CreateSubmissionRequest
    {   
        [Required(ErrorMessage = "Task assignment id is required")]
        public int TaskAssignmentId { get; set; }

        [Required( ErrorMessage = "Url is required" )]
        [Url]
        public string SubmissionUrl { get; set; } = string.Empty ;

        public string Notes { get; set; } = string.Empty ;

        public DateTime SubmittedDate { get; set; } = DateTime.Now ;

        public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;

    }
}
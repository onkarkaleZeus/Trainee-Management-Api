using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.TaskAssignmentDto
{
    public class CreateTaskAssignmentRequest
    {

        [Required( ErrorMessage = "Trainee id is required" )]
        public int TraineeId { get; set; }

        [Required( ErrorMessage = "Mentor id is required" )]
        public int MentorId { get; set; }

        [Required( ErrorMessage = "Learning Task id is required" )]
        public int LearningTaskId { get; set; }

        [Required( ErrorMessage = "Task assigned date is required" )]
        public DateTime AssignedDate { get; set; } = DateTime.Now ;

        [Required( ErrorMessage = "Task Due date is required" )]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(5) ;

        [Required( ErrorMessage = "Task assignment status is required" )]
        public TaskAssignmentStatus Status { get; set; } = TaskAssignmentStatus.Assigned;

        public string? Remarks { get; set; } = string.Empty ;
        
    }
}
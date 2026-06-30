using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.TaskAssignmentDto
{
    public class UpdateTaskAssignmentRequest
    {

        [Required( ErrorMessage = "Task assignment status is required" )]
        public TaskAssignmentStatus Status { get; set; } = TaskAssignmentStatus.Assigned;
        
    }
}
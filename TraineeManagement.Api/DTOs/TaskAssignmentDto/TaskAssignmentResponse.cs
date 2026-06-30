using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.TaskAssignmentDto

{
    public class TaskAssignmentResponse
    {
        public int Id { get; set; }

        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty ;

        public int MentorId { get; set; }
        public string MentorName { get; set; } = string.Empty ;

        public int LearningTaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty ;
        public string TaskDescription { get; set; } = string.Empty ;

        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; } 

        public TaskAssignmentStatus Status { get; set; } = TaskAssignmentStatus.Assigned;

        public string Remarks { get; set; } = string.Empty ;

        public DateTime CreatedDate { get; set; } = DateTime.Now ;
        public DateTime UpdatedDate { get; set; } = DateTime.Now ;
    }
}
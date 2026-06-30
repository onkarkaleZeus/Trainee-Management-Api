using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace TraineeManagement.Api.Models
{
    public class TaskAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Trainee))]
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; } = null! ;

        [ForeignKey(nameof(Mentor))]
        public int MentorId { get; set; }
        public Mentor Mentor { get; set; } = null! ;

        [ForeignKey(nameof(LearningTask))]
        public int LearningTaskId { get; set; }
        public LearningTask LearningTask { get; set; } = null! ;

        public DateTime AssignedDate { get; set; } = DateTime.Now ;
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(5) ;

        public TaskAssignmentStatus Status { get; set; } = TaskAssignmentStatus.Assigned;

        public string Remarks { get; set; } = string.Empty ;

        public DateTime CreatedAt { get; set; } = DateTime.Now ;
        public DateTime UpdatedAt { get; set; } = DateTime.Now ;

    }

    public enum TaskAssignmentStatus
    {
        Assigned,
        InProgress,
        Submitted,
        Reviewed,
        Completed
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using TraineeManagement.Api.DTOs.TaskAssignmentDto;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.SubmissionDto
{
    public class SubmissionResponse
    {
        public int Id { get; set; } 
        public int TaskAssignmentId { get; set; }
        public DateTime TaskAssignmentDate { get; set; }
        public DateTime TaskDueDate { get; set; }
        public TaskAssignmentStatus TaskAssignmentStatus { get; set; } = TaskAssignmentStatus.Assigned;

        public string SubmissionUrl { get; set; } = string.Empty ;

        public string Notes { get; set; } = string.Empty ;

        public DateTime SubmittedDate { get; set; } = DateTime.Now ;

        public DateTime CreatedDate { get; set; } = DateTime.Now ;
        public DateTime UpdatedDate { get; set; } = DateTime.Now ;

    }
}
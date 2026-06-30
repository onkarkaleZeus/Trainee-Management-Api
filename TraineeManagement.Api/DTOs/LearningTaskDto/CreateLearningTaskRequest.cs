using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.LearningTaskDto
{
    public class CreateLearningTaskRequest
    {
        [Required(ErrorMessage = "Title is required.")][MaxLength(100)]
        public string Title { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty ;

        [Required(ErrorMessage = "ExpectedTechStack is required.")]
        public string ExpectedTechStack { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Due date is required.")]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(5) ;

        [Required(ErrorMessage = "Status is required.")]
        public LearningTaskStatus Status { get; set; } = LearningTaskStatus.Draft ;
    }
}
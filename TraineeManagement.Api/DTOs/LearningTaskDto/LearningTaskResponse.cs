using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.LearningTaskDto
{
    public class LearningTaskResponse
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")][MaxLength(100)]
        public string Title { get; set; } = "" ;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = "" ;

        [Required(ErrorMessage = "ExpectedTechStack is required.")]
        public string ExpectedTechStack { get; set; } = "" ;

        [Required(ErrorMessage = "Due date is required.")]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(5) ;

        [Required(ErrorMessage = "Status is required.")]
        public LearningTaskStatus Status { get; set; } = LearningTaskStatus.Draft ;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow ;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow ;
    }
}
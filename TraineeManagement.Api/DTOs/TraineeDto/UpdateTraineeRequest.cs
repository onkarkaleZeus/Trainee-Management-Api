using TraineeManagement.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Api.DTOs.TraineeDto
{
    public class UpdateTraineeRequest
    {
        [Required(ErrorMessage = "Firstname is required.")]
        [MaxLength(50)]
        public string? FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lastname is required.")]
        [MaxLength(50)]
        public string? LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tech Stack is required.")]
        public string? TechStack { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        public TraineeStatus Status { get; set; } = TraineeStatus.Active;
    }
}
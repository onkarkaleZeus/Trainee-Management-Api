using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.TraineeDto
{
    public class CreateTraineeRequest
    {

        [Required(ErrorMessage = "FirstName is required.")]
        [MaxLength(50)]
        public string FirstName { get; set; } = "" ;

        [Required(ErrorMessage = "LastName is required.")]
        [MaxLength(50)]
        public string LastName { get; set; } = "" ;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = "" ;

        [Required(ErrorMessage = "Tech Stack is required.")]
        public string TechStack { get; set; } = "" ;

        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(TraineeStatus), ErrorMessage = "Valid Enum value is required")]
        public TraineeStatus Status { get; set; }
    }

}
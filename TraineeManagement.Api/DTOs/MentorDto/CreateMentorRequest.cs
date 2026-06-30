using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.MentorDto
{
    public class CreateMentorRequest
    {
        [Required(ErrorMessage = "Firstname is Required")]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Lastname is Required")]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Expertise is required")]
        public string Expertise { get; set; } = string.Empty ;

        [Required(ErrorMessage = "Mentor Status is required")]
        public MentorStatus Status { get; set; } = MentorStatus.Active;
    }
}
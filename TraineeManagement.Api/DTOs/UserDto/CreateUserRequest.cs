using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.UserDto
{
    public class CreateUserRequest
    {

        [Required(ErrorMessage = "User name is required")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required.")]
        public string PasswordHash { get; set; } = "";

        [Required(ErrorMessage = "Role is required.")]
        public UserRole Role { get; set; } = UserRole.Trainee;
    }
}
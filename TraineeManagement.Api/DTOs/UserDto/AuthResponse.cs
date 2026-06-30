using Microsoft.AspNetCore.Identity;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.UserDto
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }

        public UserResponse User { get; set; }

    }
}
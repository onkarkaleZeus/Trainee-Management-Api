using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs.UserDto
{
    public class UserResponse {
        public int Id { get; set; }

        public string Username { get; set; } = "" ;
        public UserRole Role { get; set; } = UserRole.Trainee ;

    }
}
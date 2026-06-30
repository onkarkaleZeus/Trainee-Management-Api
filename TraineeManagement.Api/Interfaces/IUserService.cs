using TraineeManagement.Api.DTOs.UserDto;

namespace TraineeManagement.Api.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponse?> LoginUser( LoginUserRequest request ) ;
    }
}
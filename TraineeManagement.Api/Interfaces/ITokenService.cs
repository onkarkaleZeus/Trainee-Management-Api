using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user) ;
    }
}
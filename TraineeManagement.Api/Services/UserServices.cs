using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs.UserDto;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services
{
    public class UserServices(
        AppDbContext context,
        ITokenService service,
        ILogger<UserServices> logger

    ) : IUserService
    {

        private readonly AppDbContext _context = context;
        private readonly ITokenService _service = service;
        private readonly ILogger<UserServices> _logger = logger;

        public async Task<AuthResponse?> LoginUser(LoginUserRequest request)
        {

            var hasher = new PasswordHasher<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username) ??
                throw new KeyNotFoundException($"User with username {request.Username} not found");

            var result = hasher.VerifyHashedPassword(request.Username, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid Username or password");
            }

            var token = _service.CreateToken(user);

            _logger.LogInformation("{Username} logged in successfully.", user.Username);
            return new AuthResponse()
            {
                Token = token,
                ExpiresIn = 3600,
                User = new UserResponse()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role
                }
            };
        }
    }
}
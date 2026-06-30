using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs.UserDto;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController(IUserService service) : ControllerBase
    {

        private readonly IUserService _service = service;

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> LoginUser([FromBody] LoginUserRequest request)
        {

            var response = await _service.LoginUser(request);

            return Ok(response);
        }
    }
}
using AppointmentManagementService.Api.Authentication;
using AppointmentManagementService.Domain.Login;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementService.Api.Controllers.v1
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (loginDto.Username == "admin" && loginDto.Password == "password") // Todo: Replace with real authentication
            {
                var token = _tokenService.GenerateToken(loginDto.Username);
                return Ok(new { Token = token });
            }
            return Unauthorized("Invalid credentials.");
        }
    }

}

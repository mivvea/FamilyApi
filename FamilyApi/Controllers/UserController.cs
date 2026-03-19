using FamilyApi.DataModels;
using FamilyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FamilyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public UserController(UserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRequest registerRequest)
        {
            var user = await _userService.CreateUserAsync(registerRequest.Name, registerRequest.Password);

            if (user == null)
                return BadRequest("User with this name already exists.");

            return Ok(new { Message = "User created successfully", UserId = user.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserRequest loginRequest)
        {
            var user = await _userService.ValidateUserCredentialsAsync(loginRequest.Name, loginRequest.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
    }
}

using FamilyApi.DataModels;
using FamilyApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpPut("edit")]
        public async Task<IActionResult> EditUser([FromBody] UserRequest editRequest)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            var updatedUser = await _userService.UpdateUserAsync(userName, editRequest.Name, editRequest.Password, editRequest.Photo);

            if (updatedUser == null)
                return BadRequest("Failed to update user. Name might already exist.");

            return Ok(new { Message = "User updated successfully", UserId = updatedUser.Id });
        }
    }
}

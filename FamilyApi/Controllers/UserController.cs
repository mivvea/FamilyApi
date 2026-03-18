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
            // Create a new user
            var user = await _userService.CreateUserAsync(registerRequest.Name, registerRequest.Password);

            return Ok(new { Message = "User created successfully", UserId = user.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserRequest loginRequest)
        {
            var user = await _userService.ValidateUserCredentialsAsync(loginRequest.Name, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }
            var token = _jwtService.GenerateJwtToken(user);

            // Create a session with the JWT token
            //var session = new Session
            //{
            //    UserId = user.Id.ToString(),
            //    Jwt = token
            //};

            // Here you would save the session to the database (optional)
            // _sessionCollection.InsertOne(session);

            return Ok(new { Token = token });
        }
    }
}

using System.Security.Claims;
using AuthAPI.Configuration;
using AuthAPI.Dto;
using AuthAPI.Models;
using AuthAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger _logger;
        private readonly IJwtAuthService _jwtAuthService;
        private readonly IUserService _userService ;

        public AuthController(ILogger<AuthController> logger, JwtSettings jwtSettings, IJwtAuthService jwtAuthService, IUserService userService)
        {
            _logger = logger;
            _jwtSettings = jwtSettings;
            _jwtAuthService = jwtAuthService;
            _userService = userService;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var loginResult = await _jwtAuthService.Login(user);
            
            return loginResult is null ? Unauthorized() : Ok(loginResult);
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserCreateDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newUser = await _jwtAuthService.Register(user);

            if (newUser is null)
            {
                return Conflict(new { error_message = "Email is already in use." });
            }

            return Ok(newUser);
        }
        
        //Testowy
        [HttpGet("GetCurrentUser")]
        [Authorize(Policy = "Bearer")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userService.GetUserByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            if (user is null)
            {
                return NotFound();
            }
            
            return user;
        }
    }
}

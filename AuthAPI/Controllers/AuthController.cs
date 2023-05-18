using System.Security.Claims;
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
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;

        public AuthController( IUserService userService, IAccountService accountService)
        {
            _userService = userService;
            _accountService = accountService;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var loginResult = await _accountService.Login(user);
            
            return loginResult is null ? Unauthorized() : Ok(loginResult);
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newUser = await _accountService.Register(user);

            if (newUser is null)
            {
                return Conflict(new { error_message = "Email is already in use." });
            }

            return Ok(newUser);
        }

        [HttpGet("current")]
        [Authorize(Policy = "Bearer")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userService.GetUserByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            if (user is null) return NotFound();
            
            return user;
        }
    }
}

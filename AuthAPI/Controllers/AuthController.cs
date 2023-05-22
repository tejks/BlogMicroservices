using System.Net;
using System.Security.Claims;
using AuthAPI.Dto;
using AuthAPI.Models;
using AuthAPI.Services;
using Core.Enums;
using Core.Services;
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
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, IAccountService accountService, IJwtService jwtService)
        {
            _userService = userService;
            _accountService = accountService;
            _jwtService = jwtService;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var loginResult = await _accountService.Login(dto);

            if (!loginResult.IsAuthenticated)
            {
                return Unauthorized(new { error_message = loginResult.Message });
            }

            return Ok(loginResult);
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
        
        [HttpPost]
        [Route("token/refresh")]
        public async Task<IActionResult> Refresh(TokenRequestModel token)
        {
            if (token is null) return BadRequest( new { error_message = "Invalid client request"});
            
            var refreshResult = await _accountService.RefreshToken(token);

            if (refreshResult is null)
            {
                return BadRequest(new { error_message = refreshResult.Message });
            }
            
            return Ok(refreshResult);
        }
        
        [HttpPost]
        [Authorize]
        [Route("token/revoke")]
        public async Task<IActionResult> Revoke(TokenRequestModel token)
        {
            var isRevoked = await _accountService.RevokeToken(token);

            if (!isRevoked)
            {
                return BadRequest(new { error_message = "Token expired."});
            }
            
            return Ok(new { message = "Token revoked." });
        }

        [HttpGet("currentUser")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userService.GetUserByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            if (user is null) return NotFound();
            
            return user;
        }
        
        [HttpPost("changeRole")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> ChangeUserRole(string email, Role role)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user is null) return NotFound(new { error_message = "User does not exist" });

            if (!Enum.IsDefined(typeof(Role), role)) return BadRequest(new { error_message = $"Role '{role}' does not exist" });

            var changed = await _userService.ChangeRole(user.Id, role);
            if (changed is null)
            {
                return Conflict(new { error_message = "Cannot change the role." });
            }
            
            return Ok(changed);
        }
    }
}

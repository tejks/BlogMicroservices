using Amazon.Auth.AccessControlPolicy;
using AuthAPI.Dto;
using AuthAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers
{
    [Route("api/password")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public PasswordController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("change")]
        [Authorize(Policy = "Bearer")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] UserChangePasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var changed = await _accountService.ChangePassword(id, dto);

            if (changed is null)
            {
                return Conflict(new { error_message = "Cannot change password" });
            }

            return Ok(changed);
        }

        [HttpPost("recover")]
        [Authorize(Policy = "Bearer")]
        public async Task<IActionResult> Recover(Guid id)
        {
            return Ok();
        }
    }
}

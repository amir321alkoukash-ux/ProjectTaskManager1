using Microsoft.AspNetCore.Mvc;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
                return Unauthorized("Invalid email or password");

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgottenPasswordDto forgotPasswordDto)
        {
            var token = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
            if (token == null)
                return BadRequest("User not found");

            // In production, send token via email
            return Ok(new { Token = token, Message = "Password reset token generated. Check your email (simulated)." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var success = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!success)
                return BadRequest("Invalid token or email");

            return Ok("Password reset successful");
        }
    }
}
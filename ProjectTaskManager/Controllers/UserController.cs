using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManager.Models.Account;
using ProjectTaskManager.Services.Interfaces;
using ProjectTaskManager.Services.Logging;
using System.Security.Claims;
using System.Web;
using ProjectTaskManager.DTOs; 

namespace ProjectTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILoggerManager _logger;
        // Add this field
        private readonly IEmailService _emailService;

        // Update constructor to include IEmailService
        public UserController(IUserService userService, ILoggerManager logger, IEmailService emailService)
        {
            _userService = userService;
            _logger = logger;
            _emailService = emailService;
        }

        // GET: api/user/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var profile = await _userService.GetUserProfileAsync(userId.Value);
                if (profile == null)
                {
                    return NotFound("User not found");
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var profile = await _userService.GetUserProfileAsync(id);
                if (profile == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/user
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/user/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _userService.EmailExistsAsync(model.Email))
                {
                    return BadRequest(new { error = "Email already registered" });
                }

                var result = await _userService.RegisterAsync(model, "System");

                if (result.Succeeded)
                {
                    _logger.LogInfo($"New user registered: {model.Email}");
                    return Ok(new { message = "Registration successful" });
                }

                return BadRequest(result.Errors.Select(e => e.Description));
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/user/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userService.LoginAsync(model);

                if (result.Succeeded)
                {
                    var user = await _userService.GetUserByEmailAsync(model.Email);
                    var profile = await _userService.GetUserProfileAsync(user!.Id);

                    return Ok(new
                    {
                        message = "Login successful",
                        user = profile
                    });
                }

                if (result.IsLockedOut)
                {
                    return BadRequest(new { error = "Account locked out" });
                }

                return Unauthorized(new { error = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/user/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _userService.LogoutAsync();
                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/user/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileViewModel model)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userService.UpdateUserProfileAsync(
                    userId.Value,
                    model,
                    User.Identity?.Name ?? "System");

                if (result.Succeeded)
                {
                    return Ok(new { message = "Profile updated successfully" });
                }
                return BadRequest(result.Errors.Select(e => e.Description)); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/user/changepassword
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userService.ChangePasswordAsync(
                    userId.Value,
                    model.CurrentPassword,
                    model.NewPassword);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Password changed successfully" });
                }

                return BadRequest(result.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/user/{id}/deactivate
        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                var result = await _userService.DeactivateUserAsync(id, User.Identity?.Name ?? "System");

                if (result.Succeeded)
                {
                    return Ok(new { message = "User deactivated successfully" });
                }

                return BadRequest(result.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating user ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/user/{id}/activate
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                var result = await _userService.ActivateUserAsync(id, User.Identity?.Name ?? "System");

                if (result.Succeeded)
                {
                    return Ok(new { message = "User activated successfully" });
                }

                return BadRequest(result.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating user ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/user/{id}/role
        [HttpPost("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddToRole(int id, [FromBody] string role)
        {
            try
            {
                var result = await _userService.AddToRoleAsync(id, role);

                if (result.Succeeded)
                {
                    return Ok(new { message = $"User added to role {role} successfully" });
                }

                return BadRequest(result.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding user {id} to role {role}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/user/{id}/role
        [HttpDelete("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveFromRole(int id, [FromBody] string role)
        {
            try
            {
                var result = await _userService.RemoveFromRoleAsync(id, role);

                if (result.Succeeded)
                {
                    return Ok(new { message = $"User removed from role {role} successfully" });
                }

                return BadRequest(result.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing user {id} from role {role}");
                return StatusCode(500, "Internal server error");
            }
        }

        // ========== NEW PASSWORD RESET METHODS ==========
        // POST: api/user/forgot-password
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.GetUserByEmailAsync(model.Email);
                
                // Don't reveal if user exists or not
                if (user == null)
                {
                    _logger.LogInfo($"Password reset attempted for non-existent email: {model.Email}");
                    return Ok(new { message = "If your email is registered, you will receive a password reset link." });
                }

                // Generate password reset token
                var token = await _userService.GeneratePasswordResetTokenAsync(user.Id);
                
                // Create reset link (frontend URL)
                var resetLink = $"https://localhost:7209/reset-password?email={HttpUtility.UrlEncode(model.Email)}&token={HttpUtility.UrlEncode(token)}";
                
                // Send email
                await _emailService.SendPasswordResetEmailAsync(model.Email, resetLink);
                
                _logger.LogInfo($"Password reset email sent to: {model.Email}");
                return Ok(new { message = "If your email is registered, you will receive a password reset link." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in forgot password for email: {model.Email}");
                return StatusCode(500, new { message = "An error occurred processing your request" });
            }
        }

        // POST: api/user/reset-password
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userService.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogInfo($"Password reset attempted for non-existent email: {model.Email}");
                    return BadRequest(new { message = "Invalid password reset request" });
                }

                var result = await _userService.ResetPasswordAsync(user.Id, model.Token, model.NewPassword);
                
                if (result.Succeeded)
                {
                    _logger.LogInfo($"Password reset successful for user: {model.Email}");
                    return Ok(new { message = "Password has been reset successfully" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resetting password for email: {model.Email}");
                return StatusCode(500, new { message = "An error occurred processing your request" });
            }
        }

        // GET: api/user/validate-reset-token
        [HttpGet("validate-reset-token")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateResetToken([FromQuery] string email, [FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { valid = false, message = "Invalid token" });
                }

                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return BadRequest(new { valid = false, message = "Invalid token" });
                }

                var isValid = await _userService.ValidatePasswordResetTokenAsync(user.Id, token);
                
                return Ok(new { valid = isValid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating reset token");
                return StatusCode(500, new { valid = false, message = "An error occurred" });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
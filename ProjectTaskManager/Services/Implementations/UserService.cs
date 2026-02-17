using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManager.Data;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Models.Account;
using ProjectTaskManager.Services.Interfaces;
using ProjectTaskManager.Services.Logging;

namespace ProjectTaskManager.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILoggerManager _logger;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            ApplicationDbContext context,
            ILoggerManager logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            try
            {
                _logger.LogInfo($"User attempting login: {model.Email}");

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning($"Login failed - user not found: {model.Email}");
                    return SignInResult.Failed;
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning($"Login failed - user inactive: {model.Email}");
                    return SignInResult.LockedOut;
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInfo($"User logged in successfully: {model.Email}");

                    user.UpdatedAt = DateTime.UtcNow;
                    user.UpdatedBy = user.Email;
                    await _userManager.UpdateAsync(user);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for: {model.Email}");
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInfo("User logged out");
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model, string createdBy)
        {
            try
            {
                _logger.LogInfo($"Registering new user: {model.Email}");

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CompanyId = model.CompanyId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInfo($"User registered successfully: {model.Email}");

                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<int>("User"));
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error registering user: {model.Email}");
                throw;
            }
        }

        public async Task<UserProfileViewModel?> GetUserProfileAsync(int userId)
        {
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning($"User not found with ID: {userId}");
                    return null;
                }

                var roles = await _userManager.GetRolesAsync(user);

                return new UserProfileViewModel
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CompanyId = user.CompanyId,
                    CompanyName = user.Company?.Name,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive,
                    Roles = roles.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user profile for ID: {userId}");
                throw;
            }
        }

        public async Task<IEnumerable<UserProfileViewModel>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users
                    .Include(u => u.Company)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToListAsync();

                var userProfiles = new List<UserProfileViewModel>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userProfiles.Add(new UserProfileViewModel
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CompanyId = user.CompanyId,
                        CompanyName = user.Company?.Name,
                        CreatedAt = user.CreatedAt,
                        IsActive = user.IsActive,
                        Roles = roles.ToList()
                    });
                }

                return userProfiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> UpdateUserProfileAsync(int userId, UserProfileViewModel model, string updatedBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.CompanyId = model.CompanyId;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = updatedBy;

                return await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user profile for ID: {userId}");
                throw;
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing password for user ID: {userId}");
                throw;
            }
        }

        public async Task<IdentityResult> DeactivateUserAsync(int userId, string deactivatedBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                user.IsActive = false;
                user.InactiveDate = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = deactivatedBy;

                return await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating user ID: {userId}");
                throw;
            }
        }

        public async Task<IdentityResult> ActivateUserAsync(int userId, string activatedBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                user.IsActive = true;
                user.InactiveDate = null;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = activatedBy;

                return await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating user ID: {userId}");
                throw;
            }
        }

        public async Task<IdentityResult> AddToRoleAsync(int userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>(role));
                }

                return await _userManager.AddToRoleAsync(user, role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding user {userId} to role {role}");
                throw;
            }
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(int userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found" });
                }

                return await _userManager.RemoveFromRoleAsync(user, role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing user {userId} from role {role}");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new List<string>();
            }

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsInRoleAsync(int userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _userManager.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(u => u.Email == email);
        }
        // ... all your other methods ...

        public async Task<string> GeneratePasswordResetTokenAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(int userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<bool> ValidatePasswordResetTokenAsync(int userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }
            return await _userManager.VerifyUserTokenAsync(user,
                _userManager.Options.Tokens.PasswordResetTokenProvider,
                "ResetPassword", token);
        }
    }
}
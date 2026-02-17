using Microsoft.AspNetCore.Identity;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Models.Account;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface IUserService
    {
        // User profile methods
        Task<UserProfileViewModel> GetUserProfileAsync(int userId);
        Task<IEnumerable<UserProfileViewModel>> GetAllUsersAsync();
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);

        // Registration and authentication
        Task<IdentityResult> RegisterAsync(RegisterViewModel model, string createdBy);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();

        // User management
        Task<IdentityResult> UpdateUserProfileAsync(int userId, UserProfileViewModel model, string updatedBy);
        Task<IdentityResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<IdentityResult> DeactivateUserAsync(int userId, string deactivatedBy);
        Task<IdentityResult> ActivateUserAsync(int userId, string activatedBy);

        // Role management
        Task<IdentityResult> AddToRoleAsync(int userId, string role);
        Task<IdentityResult> RemoveFromRoleAsync(int userId, string role);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);

        // ===== PASSWORD RESET METHODS =====
        Task<string> GeneratePasswordResetTokenAsync(int userId);
        Task<IdentityResult> ResetPasswordAsync(int userId, string token, string newPassword);
        Task<bool> ValidatePasswordResetTokenAsync(int userId, string token); 
    }
}
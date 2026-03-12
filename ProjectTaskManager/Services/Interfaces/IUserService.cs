using ProjectTaskManager.DTOs;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(string id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(UserDto userDto, string password, string role);
        Task UpdateUserAsync(string id, UserDto userDto);
        Task DeleteUserAsync(string id);
        Task<UserDto> GetCurrentUserAsync(string userId);
    }
}
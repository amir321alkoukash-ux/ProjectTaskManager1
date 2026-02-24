using ProjectTaskManager.DTOs;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskDto>> GetAllTasksAsync();
        Task<TaskDto> CreateTaskAsync(TaskDto taskDto);
        Task UpdateTaskAsync(int id, TaskDto taskDto);
        Task DeleteTaskAsync(int id);
        Task<IEnumerable<TaskDto>> GetTasksByUserAsync(string userId);
    }
}
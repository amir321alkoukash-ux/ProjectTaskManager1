using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<Task>> GetAllTasksAsync();
        Task<Task?> GetTaskByIdAsync(int id);
        Task<Task> CreateTaskAsync(Task task);
        Task<Task> UpdateTaskAsync(Task task);
        Task<bool> DeleteTaskAsync(int id);
        Task<bool> TaskExistsAsync(int id);
        Task<IEnumerable<Task>> GetTasksByProjectIdAsync(int projectId);
        Task<IEnumerable<Task>> GetTasksByEmployeeIdAsync(int employeeId);
        Task<bool> AssignTaskToEmployeeAsync(int taskId, int employeeId);
        Task<bool> CompleteTaskAsync(int taskId);
    }
}   
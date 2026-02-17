using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskRecord>> GetAllTasksAsync();
        Task<TaskRecord?> GetTaskByIdAsync(int id);
        Task<TaskRecord> CreateTaskAsync(TaskRecord task);
        Task<TaskRecord> UpdateTaskAsync(TaskRecord task, string name, string description, string username);
        Task<bool> DeleteTaskAsync(TaskRecord task, string username);
        Task<bool> TaskExistsAsync(int id);
        Task<bool> TaskNameExistsAsync(string name, int projectId);
        Task<IEnumerable<TaskRecord>> GetTasksByProjectIdAsync(int projectId);
        Task<IEnumerable<TaskRecord>> GetTasksByEmployeeIdAsync(int employeeId);
        Task<bool> AssignTaskToEmployeeAsync(int taskId, int employeeId);
        Task<bool> MarkTaskAsCompletedAsync(int taskId, string username);
        void SaveChanges();
    }
}
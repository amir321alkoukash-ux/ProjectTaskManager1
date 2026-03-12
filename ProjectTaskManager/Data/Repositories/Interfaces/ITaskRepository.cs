using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Data.Repositories.Interfaces
{
    public interface ITaskRepository : IRepository<TaskRecord>
    {
        Task<IEnumerable<TaskRecord>> GetTasksByUserAsync(string userId);
    }
}
using Microsoft.EntityFrameworkCore;
using ProjectTaskManager.Data.Context;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Data.Repositories.Interfaces;

namespace ProjectTaskManager.Data.Repositories.Implementations
{
    public class TaskRepository : Repository<TaskRecord>, ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskRecord>> GetTasksByUserAsync(string userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .Include(t => t.Project)
                .ToListAsync();
        }
    }
}
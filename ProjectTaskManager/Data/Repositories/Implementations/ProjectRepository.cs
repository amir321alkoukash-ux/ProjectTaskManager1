using Microsoft.EntityFrameworkCore;
using ProjectTaskManager.Data.Context;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Data.Repositories.Interfaces;

namespace ProjectTaskManager.Data.Repositories.Implementations
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Project?> GetProjectWithDetailsAsync(int projectId)
        {
            return await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.Tasks)
                .Include(p => p.ProjectEmployees)
                    .ThenInclude(pe => pe.User)
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }
    }
}
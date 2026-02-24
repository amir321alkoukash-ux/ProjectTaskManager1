using ProjectTaskManager.Entities;


namespace ProjectTaskManager.Data.Repositories.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project?> GetProjectWithDetailsAsync(int projectId);
    }
}
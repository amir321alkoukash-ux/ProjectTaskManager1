using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task<Project> CreateProjectAsync(Project project);
        Task<Project> UpdateProjectAsync(Project project);
        Task<bool> DeleteProjectAsync(int id);
        Task<bool> ProjectExistsAsync(int id);
        Task<IEnumerable<Project>> GetProjectsByCompanyIdAsync(int companyId);
        Task<bool> AssignEmployeeToProjectAsync(int projectId, int employeeId);
        Task<bool> RemoveEmployeeFromProjectAsync(int projectId, int employeeId);
        Task<IEnumerable<Employee>> GetProjectEmployeesAsync(int projectId);
    }
}
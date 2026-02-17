using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface IProjectService 
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task<Project> CreateProjectAsync(Project project);
        Task<Project> UpdateProjectAsync(Project project, string name, string description, string username);
        Task<bool> DeleteProjectAsync(Project project, string username);
        Task<bool> ProjectExistsAsync(int id);
        Task<bool> ProjectNameExistsAsync(string name, int companyId);
        Task<IEnumerable<Project>> GetProjectsByCompanyIdAsync(int companyId);
        Task<IEnumerable<Employee>> GetProjectEmployeesAsync(int projectId);
        Task<bool> AssignEmployeeToProjectAsync(int projectId, int employeeId);
        Task<bool> RemoveEmployeeFromProjectAsync(int projectId, int employeeId);
        Task<int> GetProjectTaskCountAsync(int projectId);
        Task<int> GetProjectEmployeeCountAsync(int projectId);
        void SaveChanges();
    }
}
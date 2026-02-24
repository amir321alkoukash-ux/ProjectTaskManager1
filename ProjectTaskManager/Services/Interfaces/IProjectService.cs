using ProjectTaskManager.DTOs;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectDto> GetProjectByIdAsync(int id);
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto);
        Task UpdateProjectAsync(int id, ProjectDto projectDto);
        Task DeleteProjectAsync(int id);
        Task AssignUserToProjectAsync(int projectId, string userId);
        Task RemoveUserFromProjectAsync(int projectId, string userId);
    }
}
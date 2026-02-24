using AutoMapper;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Services.Implementations
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IRepository<ProjectEmployee> _projectEmployeeRepository;
        private readonly IMapper _mapper;

        public ProjectService(
            IProjectRepository projectRepository,
            IRepository<ProjectEmployee> projectEmployeeRepository,
            IMapper mapper)
        {
            _projectRepository = projectRepository;
            _projectEmployeeRepository = projectEmployeeRepository;
            _mapper = mapper;
        }

        public async Task<ProjectDto> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetProjectWithDetailsAsync(id);
            if (project == null) throw new KeyNotFoundException("Project not found");
            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto)
        {
            var project = _mapper.Map<Project>(projectDto);
            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();
            return _mapper.Map<ProjectDto>(project);
        }

        public async Task UpdateProjectAsync(int id, ProjectDto projectDto)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null) throw new KeyNotFoundException("Project not found");
            _mapper.Map(projectDto, project);
            _projectRepository.Update(project);
            await _projectRepository.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project != null)
            {
                _projectRepository.Remove(project);
                await _projectRepository.SaveChangesAsync();
            }
        }

        public async Task AssignUserToProjectAsync(int projectId, string userId)
        {
            var exists = await _projectEmployeeRepository.AnyAsync(pe => pe.ProjectId == projectId && pe.UserId == userId);
            if (!exists)
            {
                var projectEmployee = new ProjectEmployee { ProjectId = projectId, UserId = userId };
                await _projectEmployeeRepository.AddAsync(projectEmployee);
                await _projectEmployeeRepository.SaveChangesAsync();
            }
        }

        public async Task RemoveUserFromProjectAsync(int projectId, string userId)
        {
            var pe = await _projectEmployeeRepository.FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.UserId == userId);
            if (pe != null)
            {
                _projectEmployeeRepository.Remove(pe);
                await _projectEmployeeRepository.SaveChangesAsync();
            }
        }
    }
}
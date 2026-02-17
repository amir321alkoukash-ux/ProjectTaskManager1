using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Services.Implementations
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<ProjectEmployee> _projectEmployeeRepository;
        private readonly IRepository<TaskRecord> _taskRepository;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(
            IRepository<Project> projectRepository,
            IRepository<Company> companyRepository,
            IRepository<ProjectEmployee> projectEmployeeRepository,
            IRepository<TaskRecord> taskRepository,
            ILogger<ProjectService> logger)
        {
            _projectRepository = projectRepository;
            _companyRepository = companyRepository;
            _projectEmployeeRepository = projectEmployeeRepository;
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all projects");
                return await _projectRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all projects");
                throw;
            }
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting project with ID: {Id}", id);
                return await _projectRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project with ID: {Id}", id);
                throw;
            }
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            try
            {
                _logger.LogInformation("Creating new project: {Name}", project.Name);
                await _projectRepository.AddAsync(project);
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                throw;
            }
        }

        public async Task<Project> UpdateProjectAsync(Project project, string name, string description, string username)
        {
            try
            {
                _logger.LogInformation("Updating project with ID: {Id}", project.Id);
                project.Name = name;
                project.Description = description;
                project.UpdatedBy = username;
                project.UpdatedAt = DateTime.UtcNow;

                _projectRepository.Update(project);
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project with ID: {Id}", project.Id);
                throw;
            }
        }

        public async Task<bool> DeleteProjectAsync(Project project, string username)
        {
            try
            {
                _logger.LogInformation("Deleting project with ID: {Id}", project.Id);
                project.UpdatedBy = username;
                project.InactiveDate = DateTime.UtcNow;
                project.UpdatedAt = DateTime.UtcNow;
                _projectRepository.Update(project);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project with ID: {Id}", project.Id);
                throw;
            }
        }

        public async Task<bool> ProjectExistsAsync(int id)
        {
            return await _projectRepository.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> ProjectNameExistsAsync(string name, int companyId)
        {
            return await _projectRepository.AnyAsync(p => p.Name == name && p.CompanyId == companyId);
        }

        public async Task<IEnumerable<Project>> GetProjectsByCompanyIdAsync(int companyId)
        {
            try
            {
                _logger.LogInformation("Getting projects for company ID: {CompanyId}", companyId);
                return await _projectRepository.FindAsync(p => p.CompanyId == companyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects for company ID: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetProjectEmployeesAsync(int projectId)
        {
            // Implementation needed - get employees through ProjectEmployee relationship
            return new List<Employee>();
        }

        public async Task<bool> AssignEmployeeToProjectAsync(int projectId, int employeeId)
        {
            try
            {
                var assignment = new ProjectEmployee
                {
                    ProjectId = projectId,
                    EmployeeId = employeeId,
                    AssignedDate = DateTime.UtcNow
                };

                // Check if already assigned
                var existing = await _projectEmployeeRepository
                    .FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);

                if (existing != null)
                    return false;

                await _projectEmployeeRepository.AddAsync(assignment);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning employee {EmployeeId} to project {ProjectId}",
                    employeeId, projectId);
                throw;
            }
        }

        public async Task<bool> RemoveEmployeeFromProjectAsync(int projectId, int employeeId)
        {
            try
            {
                var assignment = await _projectEmployeeRepository
                    .FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);

                if (assignment == null)
                    return false;

                _projectEmployeeRepository.Remove(assignment);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing employee {EmployeeId} from project {ProjectId}",
                    employeeId, projectId);
                throw;
            }
        }

        public async Task<int> GetProjectTaskCountAsync(int projectId)
        {
            return await _taskRepository.CountAsync(t => t.ProjectId == projectId);
        }

        public async Task<int> GetProjectEmployeeCountAsync(int projectId)
        {
            return await _projectEmployeeRepository.CountAsync(pe => pe.ProjectId == projectId);
        }

        public void SaveChanges()
        {
            _projectRepository.SaveChanges();
        }
    }
}
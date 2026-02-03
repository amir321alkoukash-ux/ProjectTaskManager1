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
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(
            IRepository<Project> projectRepository,
            IRepository<Company> companyRepository,
            IRepository<ProjectEmployee> projectEmployeeRepository,
            ILogger<ProjectService> logger)
        {
            _projectRepository = projectRepository;
            _companyRepository = companyRepository;
            _projectEmployeeRepository = projectEmployeeRepository;
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
                // Check if company exists
                var company = await _companyRepository.GetByIdAsync(project.CompanyId);
                if (company == null)
                    throw new InvalidOperationException($"Company with ID {project.CompanyId} not found.");

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

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            try
            {
                var existingProject = await _projectRepository.GetByIdAsync(project.Id);
                if (existingProject == null)
                    throw new KeyNotFoundException($"Project with ID {project.Id} not found.");

                // Check if company exists (if changed)
                if (project.CompanyId != existingProject.CompanyId)
                {
                    var company = await _companyRepository.GetByIdAsync(project.CompanyId);
                    if (company == null)
                        throw new InvalidOperationException($"Company with ID {project.CompanyId} not found.");
                }

                _logger.LogInformation("Updating project with ID: {Id}", project.Id);
                _projectRepository.Update(project);
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project with ID: {Id}", project.Id);
                throw;
            }
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null)
                    return false;

                _logger.LogInformation("Deleting project with ID: {Id}", id);
                _projectRepository.Remove(project);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ProjectExistsAsync(int id)
        {
            return await _projectRepository.AnyAsync(p => p.Id == id);
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

        public async Task<bool> AssignEmployeeToProjectAsync(int projectId, int employeeId)
        {
            try
            {
                // Check if project exists
                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project == null)
                    throw new KeyNotFoundException($"Project with ID {projectId} not found.");

                // Check if assignment already exists
                var existingAssignment = await _projectEmployeeRepository
                    .FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);
                
                if (existingAssignment != null)
                    return false; // Already assigned

                var projectEmployee = new ProjectEmployee
                {
                    ProjectId = projectId,
                    EmployeeId = employeeId,
                    AssignedDate = DateTime.UtcNow
                };

                await _projectEmployeeRepository.AddAsync(projectEmployee);
                _logger.LogInformation("Assigned employee {EmployeeId} to project {ProjectId}", employeeId, projectId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning employee {EmployeeId} to project {ProjectId}", employeeId, projectId);
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
                _logger.LogInformation("Removed employee {EmployeeId} from project {ProjectId}", employeeId, projectId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing employee {EmployeeId} from project {ProjectId}", employeeId, projectId);
                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetProjectEmployeesAsync(int projectId)
        {
            try
            {
                // This needs to be implemented with proper navigation
                // For now, returning empty list - you'll need to implement this based on your navigation properties
                _logger.LogInformation("Getting employees for project ID: {ProjectId}", projectId);
                return new List<Employee>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees for project ID: {ProjectId}", projectId);
                throw;
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ICompanyService _companyService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(
            IProjectService projectService,
            ICompanyService companyService,
            ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _companyService = companyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                var projectDtos = new List<ProjectDto>();

                foreach (var project in projects)
                {
                    var dto = new ProjectDto
                    {
                        Id = project.Id,
                        Name = project.Name,
                        Description = project.Description,
                        DueDate = project.DueDate,
                        CompanyId = project.CompanyId,
                        CompanyName = project.Company?.Name,
                        CreatedAt = project.CreatedAt,
                        IsActive = project.IsActive,
                        TaskCount = await _projectService.GetProjectTaskCountAsync(project.Id),
                        EmployeeCount = await _projectService.GetProjectEmployeeCountAsync(project.Id)
                    };
                    projectDtos.Add(dto);
                }

                return Ok(projectDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                if (project == null)
                    return NotFound();

                var projectDto = new ProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    DueDate = project.DueDate,
                    CompanyId = project.CompanyId,
                    CompanyName = project.Company?.Name,
                    CreatedAt = project.CreatedAt,
                    IsActive = project.IsActive,
                    TaskCount = await _projectService.GetProjectTaskCountAsync(project.Id),
                    EmployeeCount = await _projectService.GetProjectEmployeeCountAsync(project.Id)
                };

                return Ok(projectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if company exists
                if (!await _companyService.CompanyExistsAsync(createDto.CompanyId))
                    return NotFound($"Company with ID {createDto.CompanyId} not found");

                // Check if project name already exists in this company
                if (await _projectService.ProjectNameExistsAsync(createDto.Name, createDto.CompanyId))
                    return Conflict($"Project with name '{createDto.Name}' already exists in this company");

                var project = new Project
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    DueDate = (DateTime)createDto.DueDate,
                    CompanyId = createDto.CompanyId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdProject = await _projectService.CreateProjectAsync(project);
                _projectService.SaveChanges();

                var projectDto = new ProjectDto
                {
                    Id = createdProject.Id,
                    Name = createdProject.Name,
                    Description = createdProject.Description,
                    DueDate = createdProject.DueDate,
                    CompanyId = createdProject.CompanyId,
                    CompanyName = createdProject.Company?.Name,
                    CreatedAt = createdProject.CreatedAt,
                    IsActive = createdProject.IsActive,
                    TaskCount = 0,
                    EmployeeCount = 0
                };

                return CreatedAtAction(nameof(GetProject),
                    new { id = projectDto.Id },
                    projectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectDto>> UpdateProject(int id, UpdateProjectDto updateDto)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                if (project == null)
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // If changing company, check if new company exists
                if (updateDto.CompanyId.HasValue && updateDto.CompanyId != project.CompanyId)
                {
                    if (!await _companyService.CompanyExistsAsync(updateDto.CompanyId.Value))
                        return NotFound($"Company with ID {updateDto.CompanyId} not found");
                }

                var nameToUpdate = updateDto.Name ?? project.Name;
                var companyIdForCheck = updateDto.CompanyId ?? project.CompanyId;

                // Check if project name already exists in the company
                if (nameToUpdate != project.Name || companyIdForCheck != project.CompanyId)
                {
                    if (await _projectService.ProjectNameExistsAsync(nameToUpdate, companyIdForCheck))
                        return Conflict($"Project with name '{nameToUpdate}' already exists in this company");
                }

                var username = "System"; // Get from authentication
                var descriptionToUpdate = updateDto.Description ?? project.Description;

                var updatedProject = await _projectService.UpdateProjectAsync(
                    project, nameToUpdate, description: descriptionToUpdate, username);

                // Update other fields
                if (updateDto.DueDate.HasValue)
                    updatedProject.DueDate = updateDto.DueDate.Value;
                if (updateDto.CompanyId.HasValue)
                    updatedProject.CompanyId = updateDto.CompanyId.Value;

                _projectService.SaveChanges();

                var projectDto = new ProjectDto
                {
                    Id = updatedProject.Id,
                    Name = updatedProject.Name,
                    Description = updatedProject.Description,
                    DueDate = updatedProject.DueDate,
                    CompanyId = updatedProject.CompanyId,
                    CompanyName = updatedProject.Company?.Name,
                    CreatedAt = updatedProject.CreatedAt,
                    IsActive = updatedProject.IsActive,
                    TaskCount = await _projectService.GetProjectTaskCountAsync(updatedProject.Id),
                    EmployeeCount = await _projectService.GetProjectEmployeeCountAsync(updatedProject.Id)
                };

                return Ok(projectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                if (project == null)
                    return NotFound();

                var username = "System"; // Get from authentication
                var result = await _projectService.DeleteProjectAsync(project, username);

                if (!result)
                    return NotFound();

                _projectService.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsByCompanyId(int companyId)
        {
            try
            {
                var projects = await _projectService.GetProjectsByCompanyIdAsync(companyId);
                var projectDtos = projects.Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    DueDate = p.DueDate,
                    CompanyId = p.CompanyId,
                    CreatedAt = p.CreatedAt,
                    IsActive = p.IsActive
                });

                return Ok(projectDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects for company ID: {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{projectId}/employees/{employeeId}")]
        public async Task<IActionResult> AssignEmployeeToProject(int projectId, int employeeId)
        {
            try
            {
                var result = await _projectService.AssignEmployeeToProjectAsync(projectId, employeeId);
                if (!result)
                    return BadRequest("Unable to assign employee to project");

                _projectService.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning employee {EmployeeId} to project {ProjectId}",
                    employeeId, projectId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{projectId}/employees/{employeeId}")]
        public async Task<IActionResult> RemoveEmployeeFromProject(int projectId, int employeeId)
        {
            try
            {
                var result = await _projectService.RemoveEmployeeFromProjectAsync(projectId, employeeId);
                if (!result)
                    return BadRequest("Unable to remove employee from project");

                _projectService.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing employee {EmployeeId} from project {ProjectId}",
                    employeeId, projectId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
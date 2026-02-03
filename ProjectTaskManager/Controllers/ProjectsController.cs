using Microsoft.AspNetCore.Mvc;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                if (project == null)
                    return NotFound();

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            try
            {
                var createdProject = await _projectService.CreateProjectAsync(project);
                return CreatedAtAction(nameof(GetProject), new { id = createdProject.Id }, createdProject);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            try
            {
                if (id != project.Id)
                    return BadRequest("ID mismatch");

                var updatedProject = await _projectService.UpdateProjectAsync(project);
                return Ok(updatedProject);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
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
                var result = await _projectService.DeleteProjectAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectsByCompany(int companyId)
        {
            try
            {
                var projects = await _projectService.GetProjectsByCompanyIdAsync(companyId);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects for company ID: {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{projectId}/assign/{employeeId}")]
        public async Task<IActionResult> AssignEmployeeToProject(int projectId, int employeeId)
        {
            try
            {
                var result = await _projectService.AssignEmployeeToProjectAsync(projectId, employeeId);
                if (!result)
                    return BadRequest("Employee already assigned to this project");

                return Ok(new { message = "Employee assigned successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning employee {EmployeeId} to project {ProjectId}", employeeId, projectId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{projectId}/remove/{employeeId}")]
        public async Task<IActionResult> RemoveEmployeeFromProject(int projectId, int employeeId)
        {
            try
            {
                var result = await _projectService.RemoveEmployeeFromProjectAsync(projectId, employeeId);
                if (!result)
                    return NotFound("Assignment not found");

                return Ok(new { message = "Employee removed from project successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing employee {EmployeeId} from project {ProjectId}", employeeId, projectId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
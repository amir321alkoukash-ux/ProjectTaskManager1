using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                return Ok(project);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProjectDto dto)
        {
            var created = await _projectService.CreateProjectAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ProjectDto dto)
        {
            try
            {
                await _projectService.UpdateProjectAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }

        [HttpPost("{projectId}/assign-user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignUser(int projectId, string userId)
        {
            await _projectService.AssignUserToProjectAsync(projectId, userId);
            return Ok();
        }

        [HttpDelete("{projectId}/remove-user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUser(int projectId, string userId)
        {
            await _projectService.RemoveUserFromProjectAsync(projectId, userId);
            return NoContent();
        }
    }
}
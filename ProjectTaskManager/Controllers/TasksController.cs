using Microsoft.AspNetCore.Mvc;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> GetTask(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound();

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Task>> CreateTask(Task task)
        {
            try
            {
                var createdTask = await _taskService.CreateTaskAsync(task);
                return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, Task task)
        {
            try
            {
                if (id != task.Id)
                    return BadRequest("ID mismatch");

                var updatedTask = await _taskService.UpdateTaskAsync(task);
                return Ok(updatedTask);
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
                _logger.LogError(ex, "Error updating task with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasksByProject(int projectId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByProjectIdAsync(projectId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for project ID: {ProjectId}", projectId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasksByEmployee(int employeeId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByEmployeeIdAsync(employeeId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for employee ID: {EmployeeId}", employeeId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{taskId}/assign/{employeeId}")]
        public async Task<IActionResult> AssignTaskToEmployee(int taskId, int employeeId)
        {
            try
            {
                var result = await _taskService.AssignTaskToEmployeeAsync(taskId, employeeId);
                if (!result)
                    return BadRequest("Failed to assign task");

                return Ok(new { message = "Task assigned successfully" });
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
                _logger.LogError(ex, "Error assigning task {TaskId} to employee {EmployeeId}", taskId, employeeId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            try
            {
                var result = await _taskService.CompleteTaskAsync(id);
                if (!result)
                    return NotFound();

                return Ok(new { message = "Task marked as completed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing task with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
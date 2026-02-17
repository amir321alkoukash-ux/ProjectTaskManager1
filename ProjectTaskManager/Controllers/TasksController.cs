using Microsoft.AspNetCore.Mvc;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(
            ITaskService taskService,
            IProjectService projectService,
            IEmployeeService employeeService,
            ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _projectService = projectService;
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                var taskDtos = tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    CompletionDate = t.CompletionDate,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project?.Name,
                    EmployeeId = t.EmployeeId,
                    EmployeeName = t.Employee != null ?
                        $"{t.Employee.FirstName} {t.Employee.LastName}" : null,
                    CreatedAt = t.CreatedAt,
                    IsActive = t.IsActive,
                    
                });

                return Ok(taskDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound();

                var taskDto = new TaskDto
                {
                    Id = task.Id,
                    Name = task.Name,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    CompletionDate = task.CompletionDate,
                    ProjectId = task.ProjectId,
                    ProjectName = task.Project?.Name,
                    EmployeeId = task.EmployeeId,
                    EmployeeName = task.Employee != null ?
                        $"{task.Employee.FirstName} {task.Employee.LastName}" : null,
                    CreatedAt = task.CreatedAt,
                    IsActive = task.IsActive,
                   
                };

                return Ok(taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if project exists
                if (!await _projectService.ProjectExistsAsync(createDto.ProjectId))
                    return NotFound($"Project with ID {createDto.ProjectId} not found");

                // If assigning to employee, check if employee exists
                if (createDto.EmployeeId.HasValue &&
                    !await _employeeService.EmployeeExistsAsync(createDto.EmployeeId.Value))
                    return NotFound($"Employee with ID {createDto.EmployeeId} not found");

                // Check if task name already exists in this project
                if (await _taskService.TaskNameExistsAsync(createDto.Name, createDto.ProjectId))
                    return Conflict($"Task with name '{createDto.Name}' already exists in this project");

                var task = new TaskRecord
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    DueDate = createDto.DueDate,
                    ProjectId = createDto.ProjectId,
                    EmployeeId = createDto.EmployeeId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdTask = await _taskService.CreateTaskAsync(task);
                _taskService.SaveChanges();

                var taskDto = new TaskDto
                {
                    Id = createdTask.Id,
                    Name = createdTask.Name,
                    Description = createdTask.Description,
                    DueDate = createdTask.DueDate,
                    CompletionDate = createdTask.CompletionDate,
                    ProjectId = createdTask.ProjectId,
                    EmployeeId = createdTask.EmployeeId,
                    CreatedAt = createdTask.CreatedAt,
                    IsActive = createdTask.IsActive,
                   
                };

                return CreatedAtAction(nameof(GetTask),
                    new { id = taskDto.Id },
                    taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, UpdateTaskDto updateDto)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // If changing project, check if new project exists
                if (updateDto.ProjectId.HasValue && updateDto.ProjectId != task.ProjectId)
                {
                    if (!await _projectService.ProjectExistsAsync(updateDto.ProjectId.Value))
                        return NotFound($"Project with ID {updateDto.ProjectId} not found");
                }

                // If changing employee, check if new employee exists
                if (updateDto.EmployeeId.HasValue && updateDto.EmployeeId != task.EmployeeId)
                {
                    if (!await _employeeService.EmployeeExistsAsync(updateDto.EmployeeId.Value))
                        return NotFound($"Employee with ID {updateDto.EmployeeId} not found");
                }

                var nameToUpdate = updateDto.Name ?? task.Name;
                var descriptionToUpdate = updateDto.Description ?? task.Description;
                var username = "System"; // Get from authentication

                var updatedTask = await _taskService.UpdateTaskAsync(
                    task, nameToUpdate, description: descriptionToUpdate, username);

                // Update other fields
                if (updateDto.DueDate.HasValue)
                    updatedTask.DueDate = updateDto.DueDate.Value;
                if (updateDto.CompletionDate.HasValue)
                    updatedTask.CompletionDate = updateDto.CompletionDate.Value;
                if (updateDto.ProjectId.HasValue)
                    updatedTask.ProjectId = updateDto.ProjectId.Value;
                if (updateDto.EmployeeId.HasValue)
                    updatedTask.EmployeeId = updateDto.EmployeeId.Value;

                _taskService.SaveChanges();

                var taskDto = new TaskDto
                {
                    Id = updatedTask.Id,
                    Name = updatedTask.Name,
                    Description = updatedTask.Description,
                    DueDate = updatedTask.DueDate,
                    CompletionDate = updatedTask.CompletionDate,
                    ProjectId = updatedTask.ProjectId,
                    ProjectName = updatedTask.Project?.Name,
                    EmployeeId = updatedTask.EmployeeId,
                    EmployeeName = updatedTask.Employee != null ?
                        $"{updatedTask.Employee.FirstName} {updatedTask.Employee.LastName}" : null,
                    CreatedAt = updatedTask.CreatedAt,
                    IsActive = updatedTask.IsActive,
                   
                };

                return Ok(taskDto);
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
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound();

                var username = "System"; // Get from authentication
                var result = await _taskService.DeleteTaskAsync(task, username);

                if (!result)
                    return NotFound();

                _taskService.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByProjectId(int projectId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByProjectIdAsync(projectId);
                var taskDtos = tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    CompletionDate = t.CompletionDate,
                    ProjectId = t.ProjectId,
                    EmployeeId = t.EmployeeId,
                    CreatedAt = t.CreatedAt,
                    IsActive = t.IsActive,
                   
                });

                return Ok(taskDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for project ID: {ProjectId}", projectId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByEmployeeId(int employeeId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByEmployeeIdAsync(employeeId);
                var taskDtos = tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    CompletionDate = t.CompletionDate,
                    ProjectId = t.ProjectId,
                    EmployeeId = t.EmployeeId,
                    CreatedAt = t.CreatedAt,
                    IsActive = t.IsActive,
                    
                });

                return Ok(taskDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for employee ID: {EmployeeId}", employeeId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{taskId}/assign/{employeeId}")]
        public async Task<IActionResult> AssignTaskToEmployee(int taskId, int employeeId)
        {
            try
            {
                var result = await _taskService.AssignTaskToEmployeeAsync(taskId, employeeId);
                if (!result)
                    return BadRequest("Unable to assign task to employee");

                _taskService.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task {TaskId} to employee {EmployeeId}",
                    taskId, employeeId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{taskId}/complete")]
        public async Task<IActionResult> MarkTaskAsCompleted(int taskId)
        {
            try
            {
                var username = "System"; // Get from authentication
                var result = await _taskService.MarkTaskAsCompletedAsync(taskId, username);
                if (!result)
                    return BadRequest("Unable to mark task as completed");

                _taskService.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking task {TaskId} as completed", taskId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
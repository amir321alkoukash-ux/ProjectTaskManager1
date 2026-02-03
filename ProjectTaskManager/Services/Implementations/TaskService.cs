using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<Task> _taskRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            IRepository<Task> taskRepository,
            IRepository<Project> projectRepository,
            IRepository<Employee> employeeRepository,
            ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Task>> GetAllTasksAsync()
        {
            try
            {
                _logger.LogInformation("Getting all tasks");
                return await _taskRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tasks");
                throw;
            }
        }

        public async Task<Task?> GetTaskByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting task with ID: {Id}", id);
                return await _taskRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task with ID: {Id}", id);
                throw;
            }
        }

        public async Task<Task> CreateTaskAsync(Task task)
        {
            try
            {
                // Check if project exists
                var project = await _projectRepository.GetByIdAsync(task.ProjectId);
                if (project == null)
                    throw new InvalidOperationException($"Project with ID {task.ProjectId} not found.");

                // Check if assigned employee exists (if assigned)
                if (task.EmployeeId.HasValue)
                {
                    var employee = await _employeeRepository.GetByIdAsync(task.EmployeeId.Value);
                    if (employee == null)
                        throw new InvalidOperationException($"Employee with ID {task.EmployeeId} not found.");
                }

                _logger.LogInformation("Creating new task: {Name}", task.Name);
                await _taskRepository.AddAsync(task);
                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                throw;
            }
        }

        public async Task<Task> UpdateTaskAsync(Task task)
        {
            try
            {
                var existingTask = await _taskRepository.GetByIdAsync(task.Id);
                if (existingTask == null)
                    throw new KeyNotFoundException($"Task with ID {task.Id} not found.");

                // Check if project exists (if changed)
                if (task.ProjectId != existingTask.ProjectId)
                {
                    var project = await _projectRepository.GetByIdAsync(task.ProjectId);
                    if (project == null)
                        throw new InvalidOperationException($"Project with ID {task.ProjectId} not found.");
                }

                // Check if assigned employee exists (if changed)
                if (task.EmployeeId != existingTask.EmployeeId && task.EmployeeId.HasValue)
                {
                    var employee = await _employeeRepository.GetByIdAsync(task.EmployeeId.Value);
                    if (employee == null)
                        throw new InvalidOperationException($"Employee with ID {task.EmployeeId} not found.");
                }

                _logger.LogInformation("Updating task with ID: {Id}", task.Id);
                _taskRepository.Update(task);
                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID: {Id}", task.Id);
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null)
                    return false;

                _logger.LogInformation("Deleting task with ID: {Id}", id);
                _taskRepository.Remove(task);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> TaskExistsAsync(int id)
        {
            return await _taskRepository.AnyAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Task>> GetTasksByProjectIdAsync(int projectId)
        {
            try
            {
                _logger.LogInformation("Getting tasks for project ID: {ProjectId}", projectId);
                return await _taskRepository.FindAsync(t => t.ProjectId == projectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for project ID: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<IEnumerable<Task>> GetTasksByEmployeeIdAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Getting tasks for employee ID: {EmployeeId}", employeeId);
                return await _taskRepository.FindAsync(t => t.EmployeeId == employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks for employee ID: {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<bool> AssignTaskToEmployeeAsync(int taskId, int employeeId)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(taskId);
                if (task == null)
                    throw new KeyNotFoundException($"Task with ID {taskId} not found.");

                var employee = await _employeeRepository.GetByIdAsync(employeeId);
                if (employee == null)
                    throw new InvalidOperationException($"Employee with ID {employeeId} not found.");

                task.EmployeeId = employeeId;
                _taskRepository.Update(task);

                _logger.LogInformation("Assigned task {TaskId} to employee {EmployeeId}", taskId, employeeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task {TaskId} to employee {EmployeeId}", taskId, employeeId);
                throw;
            }
        }

        public async Task<bool> CompleteTaskAsync(int taskId)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(taskId);
                if (task == null)
                    return false;

                task.CompletionDate = DateTime.UtcNow;
                _taskRepository.Update(task);

                _logger.LogInformation("Completed task with ID: {TaskId}", taskId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing task with ID: {TaskId}", taskId);
                throw;
            }
        }
    }
}
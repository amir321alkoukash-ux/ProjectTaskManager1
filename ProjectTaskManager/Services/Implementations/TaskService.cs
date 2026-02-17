using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<TaskRecord> _taskRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            IRepository<TaskRecord> taskRepository,
            IRepository<Project> projectRepository,
            IRepository<Employee> employeeRepository,
            ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskRecord>> GetAllTasksAsync()
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

        public async Task<TaskRecord?> GetTaskByIdAsync(int id)
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

        public async Task<TaskRecord> CreateTaskAsync(TaskRecord task)
        {
            try
            {
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

        public async Task<TaskRecord> UpdateTaskAsync(TaskRecord task, string name, string description, string username)
        {
            try
            {
                _logger.LogInformation("Updating task with ID: {Id}", task.Id);
                task.Name = name;
                task.Description = description;
                task.UpdatedBy = username;
                task.UpdatedAt = DateTime.UtcNow;

                _taskRepository.Update(task);
                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID: {Id}", task.Id);
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(TaskRecord task, string username)
        {
            try
            {
                _logger.LogInformation("Deleting task with ID: {Id}", task.Id);
                task.UpdatedBy = username;
                task.InactiveDate = DateTime.UtcNow;
                task.UpdatedAt = DateTime.UtcNow;
                _taskRepository.Update(task);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID: {Id}", task.Id);
                throw;
            }
        }

        public async Task<bool> TaskExistsAsync(int id)
        {
            return await _taskRepository.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> TaskNameExistsAsync(string name, int projectId)
        {
            return await _taskRepository.AnyAsync(t => t.Name == name && t.ProjectId == projectId);
        }

        public async Task<IEnumerable<TaskRecord>> GetTasksByProjectIdAsync(int projectId)
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

        public async Task<IEnumerable<TaskRecord>> GetTasksByEmployeeIdAsync(int employeeId)
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
                var task = await GetTaskByIdAsync(taskId);
                if (task == null)
                    return false;

                var employee = await _employeeRepository.GetByIdAsync(employeeId);
                if (employee == null)
                    return false;

                task.EmployeeId = employeeId;
                task.UpdatedAt = DateTime.UtcNow;
                _taskRepository.Update(task);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task {TaskId} to employee {EmployeeId}",
                    taskId, employeeId);
                throw;
            }
        }

        public async Task<bool> MarkTaskAsCompletedAsync(int taskId, string username)
        {
            try
            {
                var task = await GetTaskByIdAsync(taskId);
                if (task == null)
                    return false;

                task.CompletionDate = DateTime.UtcNow;
                task.UpdatedBy = username;
                task.UpdatedAt = DateTime.UtcNow;
                _taskRepository.Update(task);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking task {TaskId} as completed", taskId);
                throw;
            }
        }

        public void SaveChanges()
        {
            _taskRepository.SaveChanges();
        }
    }
}
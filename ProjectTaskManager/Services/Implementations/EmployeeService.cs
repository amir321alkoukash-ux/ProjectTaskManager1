using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ProjectTaskManager.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<TaskRecord> _taskRepository;
        private readonly IRepository<Project> _projectRepository; // Added
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IRepository<Employee> employeeRepository,
            IRepository<Company> companyRepository,
            IRepository<TaskRecord> taskRepository,
            IRepository<Project> projectRepository, // Added
            ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository; // Added
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            try
            {
                _logger.LogInformation("Getting all employees");
                return await _employeeRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all employees");
                throw;
            }
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting employee with ID: {Id}", id);
                return await _employeeRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee with ID: {Id}", id);
                throw;
            }
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            try
            {
                _logger.LogInformation("Creating new employee: {FirstName} {LastName}",
                    employee.FirstName, employee.LastName);
                await _employeeRepository.AddAsync(employee);
                await _employeeRepository.SaveChangesAsync(); // Added - save changes after add
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                throw;
            }
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee, string firstName, string lastName, string username)
        {
            try
            {
                _logger.LogInformation("Updating employee with ID: {Id}", employee.Id);
                employee.FirstName = firstName;
                employee.LastName = lastName;
                employee.UpdatedBy = username;
                employee.UpdatedAt = DateTime.UtcNow;

                _employeeRepository.Update(employee);
                await _employeeRepository.SaveChangesAsync(); // Changed to async
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee with ID: {Id}", employee.Id);
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(Employee employee, string username)
        {
            try
            {
                _logger.LogInformation("Deleting employee with ID: {Id}", employee.Id);
                employee.UpdatedBy = username;
                employee.InactiveDate = DateTime.UtcNow;
                employee.UpdatedAt = DateTime.UtcNow;
               
                _employeeRepository.Update(employee);
                await _employeeRepository.SaveChangesAsync(); // Changed to async
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee with ID: {Id}", employee.Id);
                throw;
            }
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _employeeRepository.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> EmployeeEmailExistsAsync(string email)
        {
            return await _employeeRepository.AnyAsync(e => e.Email == email);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByCompanyIdAsync(int companyId)
        {
            try
            {
                _logger.LogInformation("Getting employees for company ID: {CompanyId}", companyId);
                return await _employeeRepository.FindAsync(e => e.CompanyId == companyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees for company ID: {CompanyId}", companyId);
                throw;
            }
        }

       

        public async Task<IEnumerable<TaskRecord>> GetEmployeeTasksAsync(int employeeId)
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

        public async Task SaveChangesAsync() // Changed to async
        {
            await _employeeRepository.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Project>> GetEmployeeProjectsAsync(int employeeId)
        {
            throw new NotImplementedException();
        }
    }
}
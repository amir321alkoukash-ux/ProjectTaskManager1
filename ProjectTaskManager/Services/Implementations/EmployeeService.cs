using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;
namespace ProjectTaskManager.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IRepository<Employee> employeeRepository,
            IRepository<Company> companyRepository,
            ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
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
                // Check if company exists
                var company = await _companyRepository.GetByIdAsync(employee.CompanyId);
                if (company == null)
                    throw new InvalidOperationException($"Company with ID {employee.CompanyId} not found.");

                // Check if email already exists
                var existingEmployee = await _employeeRepository
                    .FirstOrDefaultAsync(e => e.Email == employee.Email);

                if (existingEmployee != null)
                    throw new InvalidOperationException($"Employee with email '{employee.Email}' already exists.");

                _logger.LogInformation("Creating new employee: {FirstName} {LastName}", employee.FirstName, employee.LastName);
                await _employeeRepository.AddAsync(employee);
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                throw;
            }
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                var existingEmployee = await _employeeRepository.GetByIdAsync(employee.Id);
                if (existingEmployee == null)
                    throw new KeyNotFoundException($"Employee with ID {employee.Id} not found.");

                // Check if company exists (if changed)
                if (employee.CompanyId != existingEmployee.CompanyId)
                {
                    var company = await _companyRepository.GetByIdAsync(employee.CompanyId);
                    if (company == null)
                        throw new InvalidOperationException($"Company with ID {employee.CompanyId} not found.");
                }

                // Check if email changed and already exists
                if (employee.Email != existingEmployee.Email)
                {
                    var emailExists = await _employeeRepository
                        .FirstOrDefaultAsync(e => employee.Email == employee.Email && e.Id != employee.Id);

                    if (emailExists != null)
                        throw new InvalidOperationException($"Employee with email '{employee.Email}' already exists.");
                }

                _logger.LogInformation("Updating employee with ID: {Id}", employee.Id);
                _employeeRepository.Update(employee);
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee with ID: {Id}", employee.Id);
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                    return false;

                _logger.LogInformation("Deleting employee with ID: {Id}", id);
                _employeeRepository.Remove(employee);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee with ID: {Id}", id);
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

        public async Task<IEnumerable<Project>> GetEmployeeProjectsAsync(int employeeId)
        {
            try
            {
                // This needs to be implemented with proper navigation
                // For now, returning empty list - you'll need to implement this based on your navigation properties
                _logger.LogInformation("Getting projects for employee ID: {EmployeeId}", employeeId);
                return new List<Project>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects for employee ID: {EmployeeId}", employeeId);
                throw;
            }
        }
    }
}
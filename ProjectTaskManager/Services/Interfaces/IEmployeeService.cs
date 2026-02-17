using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(Employee employee, string firstName, string lastName, string username);
        Task<bool> DeleteEmployeeAsync(Employee employee, string username);
        Task<bool> EmployeeExistsAsync(int id);
        Task<bool> EmployeeEmailExistsAsync(string email);
        Task<IEnumerable<Employee>> GetEmployeesByCompanyIdAsync(int companyId);
        Task<IEnumerable<Project>> GetEmployeeProjectsAsync(int employeeId);
        Task<IEnumerable<TaskRecord>> GetEmployeeTasksAsync(int employeeId);
        void SaveChanges();
    }
}
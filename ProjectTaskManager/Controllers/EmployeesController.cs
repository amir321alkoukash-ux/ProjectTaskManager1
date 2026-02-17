using Microsoft.AspNetCore.Mvc;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICompanyService _companyService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            IEmployeeService employeeService,
            ICompanyService companyService,
            ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _companyService = companyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                var employeeDtos = employees.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    DateOfBirth = e.DateOfBirth,
                    Mobile = e.Mobile,
                    CompanyId = e.CompanyId,
                    CompanyName = e.Company?.Name,
                    CreatedAt = e.CreatedAt,
                    IsActive = e.IsActive
                });

                return Ok(employeeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                    return NotFound();

                var employeeDto = new EmployeeDto
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    DateOfBirth = employee.DateOfBirth,
                    Mobile = employee.Mobile,
                    CompanyId = employee.CompanyId,
                    CompanyName = employee.Company?.Name,
                    CreatedAt = employee.CreatedAt,
                    IsActive = employee.IsActive
                };

                return Ok(employeeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if company exists
                if (!await _companyService.CompanyExistsAsync(createDto.CompanyId))
                    return NotFound($"Company with ID {createDto.CompanyId} not found");

                // Check if email already exists
                if (await _employeeService.EmployeeEmailExistsAsync(createDto.Email))
                    return Conflict($"Employee with email '{createDto.Email}' already exists");

                var employee = new Employee
                {
                    FirstName = createDto.FirstName,
                    LastName = createDto.LastName,
                    Email = createDto.Email,
                    DateOfBirth = createDto.DateOfBirth,
                    Mobile = createDto.Mobile,
                    CompanyId = createDto.CompanyId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdEmployee = await _employeeService.CreateEmployeeAsync(employee);
                _employeeService.SaveChanges();

                var employeeDto = new EmployeeDto
                {
                    Id = createdEmployee.Id,
                    FirstName = createdEmployee.FirstName,
                    LastName = createdEmployee.LastName,
                    Email = createdEmployee.Email,
                    DateOfBirth = createdEmployee.DateOfBirth,
                    Mobile = createdEmployee.Mobile,
                    CompanyId = createdEmployee.CompanyId,
                    CompanyName = createdEmployee.Company?.Name,
                    CreatedAt = createdEmployee.CreatedAt,
                    IsActive = createdEmployee.IsActive
                };

                return CreatedAtAction(nameof(GetEmployee),
                    new { id = employeeDto.Id },
                    employeeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(int id, UpdateEmployeeDto updateDto)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // If changing company, check if new company exists
                if (updateDto.CompanyId.HasValue && updateDto.CompanyId != employee.CompanyId)
                {
                    if (!await _companyService.CompanyExistsAsync(updateDto.CompanyId.Value))
                        return NotFound($"Company with ID {updateDto.CompanyId} not found");
                }

                // If changing email, check if new email exists
                if (updateDto.Email != null && updateDto.Email != employee.Email)
                {
                    if (await _employeeService.EmployeeEmailExistsAsync(updateDto.Email))
                        return Conflict($"Employee with email '{updateDto.Email}' already exists");
                }

                var firstNameToUpdate = updateDto.FirstName ?? employee.FirstName;
                var lastNameToUpdate = updateDto.LastName ?? employee.LastName;
                var username = "System"; // Get from authentication

                var updatedEmployee = await _employeeService.UpdateEmployeeAsync(
                    employee, firstNameToUpdate, lastNameToUpdate, username);

                // Update other fields
                if (updateDto.Email != null)
                    updatedEmployee.Email = updateDto.Email;
                if (updateDto.DateOfBirth.HasValue)
                    updatedEmployee.DateOfBirth = updateDto.DateOfBirth.Value;
                if (updateDto.Mobile != null)
                    updatedEmployee.Mobile = updateDto.Mobile;
                if (updateDto.CompanyId.HasValue)
                    updatedEmployee.CompanyId = updateDto.CompanyId.Value;

                _employeeService.SaveChanges();

                var employeeDto = new EmployeeDto
                {
                    Id = updatedEmployee.Id,
                    FirstName = updatedEmployee.FirstName,
                    LastName = updatedEmployee.LastName,
                    Email = updatedEmployee.Email,
                    DateOfBirth = updatedEmployee.DateOfBirth,
                    Mobile = updatedEmployee.Mobile,
                    CompanyId = updatedEmployee.CompanyId,
                    CompanyName = updatedEmployee.Company?.Name,
                    CreatedAt = updatedEmployee.CreatedAt,
                    IsActive = updatedEmployee.IsActive
                };

                return Ok(employeeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                    return NotFound();

                var username = "System"; // Get from authentication
                var result = await _employeeService.DeleteEmployeeAsync(employee, username);

                if (!result)
                    return NotFound();

                _employeeService.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByCompanyId(int companyId)
        {
            try
            {
                var employees = await _employeeService.GetEmployeesByCompanyIdAsync(companyId);
                var employeeDtos = employees.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    DateOfBirth = e.DateOfBirth,
                    Mobile = e.Mobile,
                    CompanyId = e.CompanyId,
                    CreatedAt = e.CreatedAt,
                    IsActive = e.IsActive
                });

                return Ok(employeeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees for company ID: {CompanyId}", companyId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/projects")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetEmployeeProjects(int id)
        {
            try
            {
                var projects = await _employeeService.GetEmployeeProjectsAsync(id);
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
                _logger.LogError(ex, "Error getting projects for employee ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetEmployeeTasks(int id)
        {
            try
            {
                var tasks = await _employeeService.GetEmployeeTasksAsync(id);
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
                _logger.LogError(ex, "Error getting tasks for employee ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
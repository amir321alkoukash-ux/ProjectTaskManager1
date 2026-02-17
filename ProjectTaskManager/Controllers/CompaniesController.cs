using Microsoft.AspNetCore.Mvc;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ICompanyService companyService, ILogger<CompaniesController> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies()
        {
            try
            {
                var companies = await _companyService.GetAllCompaniesAsync();
                var companyDtos = new List<CompanyDto>();

                foreach (var company in companies)
                {
                    var dto = new CompanyDto
                    {
                        Id = company.Id,
                        Name = company.Name,
                        Email = company.Email,
                        Location = company.Location,
                        CreatedAt = company.CreatedAt,
                        IsActive = company.IsActive,
                        ProjectCount = await _companyService.GetCompanyProjectCountAsync(company.Id),
                        EmployeeCount = await _companyService.GetCompanyEmployeeCountAsync(company.Id)
                    };
                    companyDtos.Add(dto);
                }

                return Ok(companyDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting companies");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> GetCompany(int id)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                if (company == null)
                    return NotFound();

                var companyDto = new CompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Email = company.Email,
                    Location = company.Location,
                    CreatedAt = company.CreatedAt,
                    IsActive = company.IsActive,
                    ProjectCount = await _companyService.GetCompanyProjectCountAsync(company.Id),
                    EmployeeCount = await _companyService.GetCompanyEmployeeCountAsync(company.Id)
                };

                return Ok(companyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CreateCompanyDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _companyService.CompanyNameExistsAsync(createDto.Name))
                    return Conflict($"Company with name '{createDto.Name}' already exists");

                var company = new Company
                {
                    Name = createDto.Name,
                    Email = createDto.Email,
                    Location = createDto.Location,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdCompany = await _companyService.CreateCompanyAsync(company);
                _companyService.SaveChanges();

                var companyDto = new CompanyDto
                {
                    Id = createdCompany.Id,
                    Name = createdCompany.Name,
                    Email = createdCompany.Email,
                    Location = createdCompany.Location,
                    CreatedAt = createdCompany.CreatedAt,
                    IsActive = createdCompany.IsActive,
                    ProjectCount = 0,
                    EmployeeCount = 0
                };

                return CreatedAtAction(nameof(GetCompany), new { id = companyDto.Id }, companyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CompanyDto>> UpdateCompany(int id, UpdateCompanyDto updateDto)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                if (company == null)
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nameToUpdate = updateDto.Name ?? company.Name;
                var emailToUpdate = updateDto.Email ?? company.Email;
                var locationToUpdate = updateDto.Location ?? company.Location;

                if (updateDto.Name != null && updateDto.Name != company.Name)
                {
                    if (await _companyService.CompanyNameExistsAsync(updateDto.Name))
                        return Conflict($"Company with name '{updateDto.Name}' already exists");
                }

                var username = "System"; // Get from authentication
                var updatedCompany = await _companyService.UpdateCompanyAsync(company, nameToUpdate, username);

                updatedCompany.Email = emailToUpdate;
                updatedCompany.Location = locationToUpdate;

                _companyService.SaveChanges();

                var companyDto = new CompanyDto
                {
                    Id = updatedCompany.Id,
                    Name = updatedCompany.Name,
                    Email = updatedCompany.Email,
                    Location = updatedCompany.Location,
                    CreatedAt = updatedCompany.CreatedAt,
                    IsActive = updatedCompany.IsActive,
                    ProjectCount = await _companyService.GetCompanyProjectCountAsync(updatedCompany.Id),
                    EmployeeCount = await _companyService.GetCompanyEmployeeCountAsync(updatedCompany.Id)
                };

                return Ok(companyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                if (company == null)
                    return NotFound();

                var username = "System"; // Get from authentication
                var result = await _companyService.DeleteCompanyAsync(company, username);

                if (!result)
                    return NotFound();

                _companyService.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company with ID: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
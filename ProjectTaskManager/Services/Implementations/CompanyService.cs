using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Services.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(
            IRepository<Company> companyRepository,
            IRepository<Project> projectRepository,
            IRepository<Employee> employeeRepository,
            ILogger<CompanyService> logger)
        {
            _companyRepository = companyRepository;
            _projectRepository = projectRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            try
            {
                _logger.LogInformation("Getting all companies");
                return await _companyRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all companies");
                throw;
            }
        }

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting company with ID: {Id}", id);
                return await _companyRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company with ID: {Id}", id);
                throw;
            }
        }

        public async Task<Company> CreateCompanyAsync(Company company)
        {
            try
            {
                _logger.LogInformation("Creating new company: {Name}", company.Name);
                await _companyRepository.AddAsync(company);
                return company;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                throw;
            }
        }

        public async Task<Company> UpdateCompanyAsync(Company company, string name, string username)
        {
            try
            {
                _logger.LogInformation("Updating company with ID: {Id}", company.Id);
                company.Name = name;
                company.UpdatedBy = username;
                company.UpdatedAt = DateTime.UtcNow;

                _companyRepository.Update(company);
                return company;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company with ID: {Id}", company.Id);
                throw;
            }
        }

        public async Task<bool> DeleteCompanyAsync(Company company, string username)
        {
            try
            {
                _logger.LogInformation("Deleting company with id: {Id}", company.Id);
                company.UpdatedBy = username;
                company.InactiveDate = DateTime.UtcNow;
                company.UpdatedAt = DateTime.UtcNow;
                _companyRepository.Update(company);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company with ID: {Id}", company.Id);
                throw;
            }
        }

        public void SaveChanges()
        {
            _companyRepository.SaveChanges();
        }

        public async Task<bool> CompanyExistsAsync(int id)
        {
            return await _companyRepository.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> CompanyNameExistsAsync(string name)
        {
            return await _companyRepository.AnyAsync(c => c.Name == name);
        }

        public async Task<int> GetCompanyProjectCountAsync(int companyId)
        {
            return await _projectRepository.CountAsync(p => p.CompanyId == companyId);
        }

        public async Task<int> GetCompanyEmployeeCountAsync(int companyId)
        {
            return await _employeeRepository.CountAsync(e => e.CompanyId == companyId);
        }
    }
}
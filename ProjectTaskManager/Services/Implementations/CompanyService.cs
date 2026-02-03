using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Services.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(IRepository<Company> companyRepository, ILogger<CompanyService> logger)
        {
            _companyRepository = companyRepository;
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
                if (await CompanyNameExistsAsync(company.Name))
                {
                    throw new InvalidOperationException($"Company with name '{company.Name}' already exists.");
                }

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

        public async Task<Company> UpdateCompanyAsync(Company company,string name,string username)

        {
                
            try
            {
               
                _logger.LogInformation("Updating company with ID: {Id}", company.Id);
                company.Name = name;
                company.UpdatedBy = username;
                company.UpdatedAt = DateTime.Now;


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
                company.InactiveDate = DateTime.Now;
                company.UpdatedAt = DateTime.Now;
                _companyRepository.Update(company);


                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company with ID: {Id}", company.Id);
                throw;
            }
        }
        public void SaveChanges() {
            _companyRepository.SaveChanges();
       
          





            // Assuming the repository pattern handles saving changes, this might be empty.
        }

        public async Task<bool> CompanyExistsAsync(int id)
        {
            return await _companyRepository.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> CompanyNameExistsAsync(string name)
        {
            return await _companyRepository.AnyAsync(c => c.Name == name);
        }

        public Task<Company> UpdateCompanyAsync(Company company)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCompanyAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
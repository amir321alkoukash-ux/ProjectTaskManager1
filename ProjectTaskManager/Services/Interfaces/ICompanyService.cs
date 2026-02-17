using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task<Company?> GetCompanyByIdAsync(int id);
        Task<Company> CreateCompanyAsync(Company company);
        Task<Company> UpdateCompanyAsync(Company company, string name, string username);
        Task<bool> DeleteCompanyAsync(Company company, string username);
        void SaveChanges();
        Task<bool> CompanyExistsAsync(int id);
        Task<bool> CompanyNameExistsAsync(string name);
        Task<int> GetCompanyProjectCountAsync(int companyId);
        Task<int> GetCompanyEmployeeCountAsync(int companyId);
    }
}
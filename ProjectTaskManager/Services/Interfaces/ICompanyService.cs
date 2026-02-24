using ProjectTaskManager.DTOs;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyDto> GetCompanyByIdAsync(int id);
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto);
        Task UpdateCompanyAsync(int id, CompanyDto companyDto);
        Task DeleteCompanyAsync(int id);
    }
}
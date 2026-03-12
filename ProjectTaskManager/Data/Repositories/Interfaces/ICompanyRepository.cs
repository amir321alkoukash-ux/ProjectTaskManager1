using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Data.Repositories.Interfaces
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<Company?> GetCompanyWithUsersAsync(int companyId);
    }
}
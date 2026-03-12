using Microsoft.EntityFrameworkCore;
using ProjectTaskManager.Data.Context;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Data.Repositories.Interfaces;

namespace ProjectTaskManager.Data.Repositories.Implementations
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Company?> GetCompanyWithUsersAsync(int companyId)
        {
            return await _context.Companies
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == companyId);
        }
    }
}
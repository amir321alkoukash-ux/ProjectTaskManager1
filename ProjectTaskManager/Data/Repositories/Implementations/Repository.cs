using Microsoft.EntityFrameworkCore;
using ProjectTaskManager.Data;
using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Entities;
using System.Linq.Expressions;

namespace ProjectTaskManager.Data.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.Where(e => e.InactiveDate == null).ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).Where(e => e.InactiveDate == null).ToListAsync();

        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).Where(e => e.InactiveDate == null).SingleOrDefaultAsync();

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).Where(e => e.InactiveDate == null).FirstOrDefaultAsync();

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
            => await _dbSet.AddRangeAsync(entities);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Remove(T entity)
            => _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities)
            => _dbSet.RemoveRange(entities);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(e => e.InactiveDate == null).AnyAsync(predicate);

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            var query = _dbSet.Where(e => e.InactiveDate == null);
            if (predicate != null)
                query = query.Where(predicate);

            return await query.CountAsync();
        }

        public async Task SaveChanges()
            => await _context.SaveChangesAsync();

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
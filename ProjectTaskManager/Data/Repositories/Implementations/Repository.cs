using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProjectTaskManager.Data.Context;
using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Entities;
using System.Collections.Generic;
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

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.SingleOrDefaultAsync(predicate);

        public async Task<T> AddAsync(T entity)
        {
            var entry= await _dbSet.AddAsync(entity);
            return entry.Entity;    

        }
        public async Task SaveChanges ()
        {
            await _context.SaveChangesAsync();  
        }

        public async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
            => predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);

        Task IRepository<T>.AddAsync(T entity)
        {
            return AddAsync(entity);
        }
    }
}
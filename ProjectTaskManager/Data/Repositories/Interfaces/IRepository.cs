using ProjectTaskManager.Entities;
using System.Linq.Expressions;

namespace ProjectTaskManager.Data.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate); 
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task SaveChangesAsync();
        void SaveChanges();

        // Add this to your ProjectTaskManager.Data.Repositories.Interfaces namespace
        public interface IProjectEmployeeRepository
        {
            Task<IEnumerable<ProjectEmployee>> FindAsync(Expression<Func<ProjectEmployee, bool>> predicate);
            Task<ProjectEmployee> AddAsync(ProjectEmployee entity);
            void Update(ProjectEmployee entity);
            void Remove(ProjectEmployee entity);
            Task SaveChangesAsync();

            Task SaveChanges();
        }
    }
}
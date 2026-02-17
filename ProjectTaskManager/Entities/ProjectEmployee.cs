using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace ProjectTaskManager.Entities
{
    public class ProjectEmployee:BaseEntity
    {
        [Key]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [Key]
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Project Project { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;
    }

    
    public interface IProjectEmployeeRepository
    {
        Task<IEnumerable<ProjectEmployee>> FindAsync(Expression<Func<ProjectEmployee, bool>> predicate);
        Task<ProjectEmployee> AddAsync(ProjectEmployee entity);
        void Update(ProjectEmployee entity);
        void Remove(ProjectEmployee entity);
        Task SaveChangesAsync();
    }




}
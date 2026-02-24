using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? InactiveDate { get; set; }

        public bool IsActive => !InactiveDate.HasValue;
    }
}
using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.Entities
{
    public class TaskRecord : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        // Foreign keys
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
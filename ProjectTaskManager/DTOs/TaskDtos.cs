using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public int? EmployeeId { get; set; }
    }

    public class UpdateTaskDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        public int? ProjectId { get; set; }
        public int? EmployeeId { get; set; }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted => CompletionDate.HasValue;
    }
}
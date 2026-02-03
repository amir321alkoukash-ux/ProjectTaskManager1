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

        [Required]
        public int ProjectId { get; set; }

        public int? AssignedToId { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "Medium";
    }

    public class UpdateTaskDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int? AssignedToId { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string? AssignedToName { get; set; }
        public int? AssignedToId { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public bool IsOverdue => DueDate.HasValue && DueDate < DateTime.UtcNow && Status != "Completed";
        public DateTime CreatedAt { get; set; }
    }

    public class TaskDetailDto : TaskDto
    {
        public ProjectDto Project { get; set; } = null!;
        public EmployeeDto? AssignedTo { get; set; }
    }
}
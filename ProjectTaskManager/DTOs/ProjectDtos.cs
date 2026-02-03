using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.DTOs
{
    public class CreateProjectDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public DateTime? DueDate { get; set; }
    }

    public class UpdateProjectDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Status { get; set; }
    }

    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TaskCount { get; set; }
        public int EmployeeCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProjectDetailDto : ProjectDto
    {
        public CompanyDto Company { get; set; } = null!;
        public List<TaskDto> Tasks { get; set; } = new();
        public List<EmployeeDto> AssignedEmployees { get; set; } = new();
    }

    public class AssignEmployeeToProjectDto
    {
        [Required]
        public int EmployeeId { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace ProjectTaskManager.DTOs
{
    public class CreateCompanyDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Location { get; set; }
    }

    public class UpdateCompanyDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Location { get; set; }
    }

    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProjectCount { get; set; }
        public int EmployeeCount { get; set; }
        public bool IsActive { get; set; }
    }

    public class CompanyDetailDto : CompanyDto
    {
        public List<ProjectDto> Projects { get; set; } = new();
        public List<EmployeeDto> Employees { get; set; } = new();
    }
}
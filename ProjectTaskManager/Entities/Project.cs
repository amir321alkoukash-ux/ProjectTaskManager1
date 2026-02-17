using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTaskManager.Entities
{
    public class Project : BaseEntity
    {


        public int Id { get; set; }


        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Budget { get; set; }
        


        // Foreign Key
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        // Navigation Properties
        public  Company Company { get; set; } = null!;
        public  ICollection<TaskRecord> Tasks { get; set; } 
        public  ICollection<ProjectEmployee> ProjectEmployees { get; set; } 
        public DateTime? DueDate { get; internal set;}
                    public Project()
        {
            Tasks = new HashSet<TaskRecord>();
            ProjectEmployees = new HashSet<ProjectEmployee>();
        }





    
}
}
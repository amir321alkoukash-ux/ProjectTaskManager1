namespace ProjectTaskManager.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int CompanyId { get; set; }
        public List<TaskDto> Tasks { get; set; }
        public List<UserDto> Users { get; set; }  // Renamed from Employees
    }
}
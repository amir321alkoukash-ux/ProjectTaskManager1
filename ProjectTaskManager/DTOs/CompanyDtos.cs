namespace ProjectTaskManager.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public List<ProjectDto> Projects { get; set; }
        public List<UserDto> Users { get; set; }  // Renamed from Employees
    }
}
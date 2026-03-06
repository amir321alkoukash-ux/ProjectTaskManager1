namespace ProjectTaskManager.DTOs
{
    public class CompanyExportDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int ProjectCount { get; set; }
        public int UserCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
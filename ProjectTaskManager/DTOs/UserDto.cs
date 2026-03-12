namespace ProjectTaskManager.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Mobile { get; set; }
        public int CompanyId { get; set; }
        public IList<string> Roles { get; set; }  // Needed for role display
    }
}
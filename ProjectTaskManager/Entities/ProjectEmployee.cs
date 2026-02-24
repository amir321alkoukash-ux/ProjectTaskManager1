namespace ProjectTaskManager.Entities
{
    public class ProjectEmployee : BaseEntity
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
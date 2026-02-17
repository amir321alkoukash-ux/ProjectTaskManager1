using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Entities;

namespace ProjectTaskManager.Services.Implementations
{
    public class ProjectServiceBase
    {
        private readonly IRepository<ProjectEmployee>? _projectEmployeeRepository;
    }
}
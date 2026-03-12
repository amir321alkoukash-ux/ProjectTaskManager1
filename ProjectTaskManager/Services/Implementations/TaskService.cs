using AutoMapper;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public TaskService(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<TaskDto> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) throw new KeyNotFoundException("Task not found");
            return _mapper.Map<TaskDto>(task);
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async Task<TaskDto> CreateTaskAsync(TaskDto taskDto)
        {
            var task = _mapper.Map<TaskRecord>(taskDto);
            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();
            return _mapper.Map<TaskDto>(task);
        }

        public async Task UpdateTaskAsync(int id, TaskDto taskDto)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) throw new KeyNotFoundException("Task not found");
            _mapper.Map(taskDto, task);
            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task != null)
            {
                _taskRepository.Remove(task);
                await _taskRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByUserAsync(string userId)
        {
            var tasks = await _taskRepository.GetTasksByUserAsync(userId);
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }
    }
}
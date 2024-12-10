using TaskManagementAPI.Models;
using TaskManagementAPI.Data;
using Task = TaskManagementAPI.Models.Task;
using TaskStatus = TaskManagementAPI.Models.TaskStatus;

namespace TaskManagementAPI.Services
{
    public class TaskService
    {
        private readonly TaskDbContext _context;
        private readonly ILogger<TaskService> _logger;

        public TaskService(TaskDbContext context, ILogger<TaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IQueryable<Task> GetTasks(string? status, DateTime? dueDate)
        {
            _logger.LogInformation("Fetching tasks with filters: Status={Status}, DueDate={DueDate}", status, dueDate);
            var query = _context.Tasks.AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TaskStatus>(status, out var parsedStatus))
            {
                query = query.Where(t => t.Status == parsedStatus);
            }

            if (dueDate.HasValue)
            {
                query = query.Where(t => t.DueDate <= dueDate.Value);
            }

            return query;
        }

        public Task? GetTaskById(int id) => _context.Tasks.Find(id);

        public void AddTask(Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        public void UpdateTask(Task task)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
        }

        public void DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
        }
    }
}

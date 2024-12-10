using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Text;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;
using Task = TaskManagementAPI.Models.Task;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly ILogger<TasksController> _logger;


        public TasksController(TaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetTasks([FromQuery] string? status, [FromQuery] DateTime? dueDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Fetching all tasks.");
            var tasks = _taskService.GetTasks(status, dueDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            return Ok(tasks);
        }
        [HttpGet("{id}")]
        public IActionResult GetTaskById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching tasks by Id.");
                var task = _taskService.GetTaskById(id);

                if (task == null)
                {
                    return NotFound(new { Message = $"Task with ID {id} not found." });
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An internal server error occurred.", Details = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid task data.", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            try
            {
                _taskService.AddTask(task);
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the task.", Details = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, Task updatedTask)
        {
            var task = _taskService.GetTaskById(id);
            if (task == null)
                return NotFound(new { Message = "Task not found" });

            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.Status = updatedTask.Status;
            task.DueDate = updatedTask.DueDate;

            _taskService.UpdateTask(task);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = _taskService.GetTaskById(id);
            if (task == null)
                return NotFound(new { Message = "Task not found" });

            _taskService.DeleteTask(id);
            return NoContent();
        }
        [HttpGet("export")]
        public IActionResult ExportTasksAsCsv([FromQuery] string? status, [FromQuery] DateTime? dueDate)
        {
            var tasks = _taskService.GetTasks(status, dueDate).ToList();

            // Generate CSV content
            var csv = new StringBuilder();
            csv.AppendLine("Id,Title,Description,Status,DueDate");

            foreach (var task in tasks)
            {
                csv.AppendLine($"{task.Id},{task.Title},{task.Description},{task.Status},{task.DueDate:yyyy-MM-dd}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "Tasks.csv");
        }
    }
}

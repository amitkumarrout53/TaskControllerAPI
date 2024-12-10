using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }

    public class Task
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Title must not exceed 100 characters.")]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        [EnumDataType(typeof(TaskStatus), ErrorMessage = "Invalid status value.")]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
    }
}

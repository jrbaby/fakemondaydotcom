using System;

namespace MondayClone.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Assignee { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskStatus Status { get; set; }

        public TaskItem() { }

        public TaskItem(int id, string title, string assignee, TaskPriority priority = TaskPriority.Medium, DateTime? dueDate = null)
        {
            Id = id;
            Title = title;
            Assignee = assignee;
            CreatedAt = DateTime.Now;
            Status = TaskStatus.ToDo;
            Priority = priority;
            DueDate = dueDate;
        }

        public int? DaysLeft()
        {
            if (!DueDate.HasValue) return null;
            return (DueDate.Value.Date - DateTime.Now.Date).Days;
        }
    }
}
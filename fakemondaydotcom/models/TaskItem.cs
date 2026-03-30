using System;

namespace MondayClone.Models
{
    public class TaskItem
    {
        public int Id { get; }
        public string Title { get; set; }
        public string Assignee { get; set; }
        public DateTime CreatedAt { get; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskStatus Status { get; set; }

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

        public override string ToString()
        {
            var due = DueDate.HasValue ? DueDate.Value.ToString("yyyy-MM-dd") : "no due";
            var daysLeft = DaysLeft();
            return $"#{Id} | {Title} | {Assignee} | {Status} | {Priority} | {due} | {daysLeft}d | {CreatedAt:g}";
        }

        public int? DaysLeft()
        {
            if (!DueDate.HasValue) return null;
            return (DueDate.Value.Date - DateTime.Now.Date).Days;
        }
    }
}
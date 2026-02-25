using System;

namespace MondayClone.Models
{
    public class TaskItem
    {
        public int Id { get; }
        public string Title { get; set; }
        public string Assignee { get; set; }
        public DateTime CreatedAt { get; }
        public TaskStatus Status { get; set; }

        public TaskItem(int id, string title, string assignee)
        {
            Id = id;
            Title = title;
            Assignee = assignee;
            CreatedAt = DateTime.Now;
            Status = TaskStatus.ToDo;
        }

        public override string ToString()
        {
            return $"#{Id} | {Title} | {Assignee} | {Status} | {CreatedAt:g}";
        }
    }
}
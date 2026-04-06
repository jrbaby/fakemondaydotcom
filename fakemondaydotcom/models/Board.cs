using System;
using System.Collections.Generic;
using System.Linq;

namespace MondayClone.Models
{
    public class Board
    {
        public string Name { get; set; } = "";
        public List<TaskItem> Tasks { get; set; } = new();
        public int NextId { get; set; } = 1;

        public Board() { }

        public Board(string name)
        {
            Name = name;
        }

        public TaskItem AddTask(string title, string assignee, TaskPriority priority = TaskPriority.Medium, DateTime? dueDate = null, TaskStatus status = TaskStatus.ToDo)
        {
            var task = new TaskItem(NextId++, title, assignee, priority, dueDate);
            task.Status = status;
            Tasks.Add(task);
            return task;
        }

        public bool RemoveTask(int id)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;
            Tasks.Remove(task);
            return true;
        }

        public bool MoveTask(int id, TaskStatus newStatus)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;
            task.Status = newStatus;
            return true;
        }

        public TaskItem? GetTask(int id) => Tasks.FirstOrDefault(t => t.Id == id);

        public List<TaskItem> GetTasksByStatus(TaskStatus status)
            => Tasks.Where(t => t.Status == status).ToList();
    }
}
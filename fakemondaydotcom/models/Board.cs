using System;
using System.Collections.Generic;
using System.Linq;

namespace MondayClone.Models
{
    public class Board
    {
        public string Name { get; }
        private readonly List<TaskItem> _tasks = new();
        private int _nextId = 1;

        public Board(string name)
        {
            Name = name;
        }

        public TaskItem AddTask(string title, string assignee, TaskPriority priority = TaskPriority.Medium, DateTime? dueDate = null, TaskStatus status = TaskStatus.ToDo)
        {
            var task = new TaskItem(_nextId++, title, assignee, priority, dueDate);
            task.Status = status;
            _tasks.Add(task);
            return task;
        }

        public bool RemoveTask(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            _tasks.Remove(task);
            return true;
        }

        public bool MoveTask(int id, TaskStatus newStatus)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            task.Status = newStatus;
            return true;
        }

        public TaskItem? GetTask(int id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public List<TaskItem> GetTasksByStatus(TaskStatus status)
        {
            return _tasks.Where(t => t.Status == status).ToList();
        }

        public void PrintBoard()
        {
            Console.WriteLine($"\n=== {Name} ===");

            PrintColumn(TaskStatus.ToDo, "TO DO");
            PrintColumn(TaskStatus.InProgress, "IN PROGRESS");
            PrintColumn(TaskStatus.Done, "DONE");
        }

        private void PrintColumn(TaskStatus status, string header)
        {
            Console.WriteLine($"\n--- {header} ---");
            var items = GetTasksByStatus(status);

            if (items.Count == 0)
            {
                Console.WriteLine("(empty)");
                return;
            }

            foreach (var task in items)
                Console.WriteLine(task);
        }
    }
}
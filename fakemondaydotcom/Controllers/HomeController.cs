using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MondayClone.Models;
using MondayClone.Services;
using TaskStatus = MondayClone.Models.TaskStatus;

namespace MondayClone.Controllers
{
    public class HomeController : Controller
    {
        private const string DataFile = "monday_data.json";

        private List<Board> LoadBoards()
        {
            var boards = Storage.Load(DataFile);
            if (boards.Count == 0)
            {
                boards.Add(new Board("Main"));
                Storage.Save(DataFile, boards);
            }
            return boards;
        }

        private void SaveBoards(List<Board> boards) => Storage.Save(DataFile, boards);

        public IActionResult Index(string? boardName)
        {
            var boards = LoadBoards();
            Board board;
            if (!string.IsNullOrWhiteSpace(boardName))
            {
                board = boards.FirstOrDefault(b => b.Name.Equals(boardName, StringComparison.OrdinalIgnoreCase)) ?? boards[0];
            }
            else
            {
                board = boards[0];
            }

            ViewData["Boards"] = boards.Select(b => b.Name).ToList();
            ViewData["SelectedBoard"] = board.Name;

            // prepare combined task list for schedule (across all boards)
            var allTasks = new List<TaskItem>();
            foreach (var b in boards)
            {
                allTasks.AddRange(b.GetTasksByStatus(TaskStatus.ToDo));
                allTasks.AddRange(b.GetTasksByStatus(TaskStatus.InProgress));
                allTasks.AddRange(b.GetTasksByStatus(TaskStatus.Done));
            }
            var ordered = allTasks.OrderBy(t => t.DueDate ?? DateTime.MaxValue).ToList();
            ViewData["AllTasks"] = ordered;

            // compute overdue and due-today lists for startup banner
            var today = DateTime.Now.Date;
            var overdue = ordered.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date < today).ToList();
            var dueToday = ordered.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == today).ToList();
            ViewData["Overdue"] = overdue;
            ViewData["DueToday"] = dueToday;

            return View(board);
        }

        [HttpPost]
        public IActionResult AddTask(string title, string assignee, int priority = (int)TaskPriority.Medium, string dueDate = "")
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(assignee))
                return RedirectToAction("Index");

            DateTime? due = null;
            if (!string.IsNullOrWhiteSpace(dueDate) && DateTime.TryParse(dueDate, out var d))
                due = d.Date;

            var boards = LoadBoards();
            // find or create a board for the assignee so each person has their own board
            var board = boards.FirstOrDefault(b => b.Name.Equals(assignee.Trim(), StringComparison.OrdinalIgnoreCase));
            if (board == null)
            {
                board = new Board(assignee.Trim());
                boards.Add(board);
            }

            board.AddTask(title.Trim(), assignee.Trim(), (TaskPriority)priority, due, TaskStatus.ToDo);
            SaveBoards(boards);

            return RedirectToAction("Index", new { boardName = board.Name });
        }

        [HttpPost]
        public IActionResult MoveTask(int id, int status, string? boardName)
        {
            var boards = LoadBoards();
            bool moved = false;
            if (!string.IsNullOrWhiteSpace(boardName))
            {
                var b = boards.FirstOrDefault(x => x.Name.Equals(boardName, StringComparison.OrdinalIgnoreCase));
                if (b != null && status >= 1 && status <= 3)
                    moved = b.MoveTask(id, (TaskStatus)status);
            }
            else
            {
                // try to find the task across boards
                foreach (var b in boards)
                {
                    if (b.MoveTask(id, (TaskStatus)status)) { moved = true; break; }
                }
            }

            SaveBoards(boards);
            return RedirectToAction("Index", new { boardName });
        }

        [HttpPost]
        public IActionResult CompleteTask(int id, string? boardName)
        {
            var boards = LoadBoards();
            if (!string.IsNullOrWhiteSpace(boardName))
            {
                var b = boards.FirstOrDefault(x => x.Name.Equals(boardName, StringComparison.OrdinalIgnoreCase));
                b?.MoveTask(id, TaskStatus.Done);
            }
            else
            {
                foreach (var b in boards)
                    if (b.MoveTask(id, TaskStatus.Done)) break;
            }
            SaveBoards(boards);
            return RedirectToAction("Index", new { boardName });
        }

        [HttpPost]
        public IActionResult DeleteTask(int id, string? boardName)
        {
            var boards = LoadBoards();
            bool removed = false;
            if (!string.IsNullOrWhiteSpace(boardName))
            {
                var b = boards.FirstOrDefault(x => x.Name.Equals(boardName, StringComparison.OrdinalIgnoreCase));
                if (b != null) removed = b.RemoveTask(id);
            }
            else
            {
                foreach (var b in boards)
                {
                    if (b.RemoveTask(id)) { removed = true; break; }
                }
            }

            SaveBoards(boards);
            return RedirectToAction("Index", new { boardName });
        }
    }
}

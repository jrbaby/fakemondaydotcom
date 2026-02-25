using System;
using System.Collections.Generic;
using System.Linq;
using MondayClone.Models;
using MondayClone.Services;

const string DataFile = "monday_data.json";

List<Board> boards = Storage.Load(DataFile);

if (boards.Count == 0)
{
    boards.Add(new Board("Main"));
    Storage.Save(DataFile, boards);
}

int currentBoardIndex = 0;

while (true)
{
    var board = boards[currentBoardIndex];

    Console.WriteLine($"\n==== MondayClone ====");
    Console.WriteLine($"Current Board: {board.Name}");
    Console.WriteLine("1) Add Task");
    Console.WriteLine("2) Move Task");
    Console.WriteLine("3) Remove Task");
    Console.WriteLine("4) View Board");
    Console.WriteLine("5) Create New Board");
    Console.WriteLine("6) Switch Board");
    Console.WriteLine("7) Save");
    Console.WriteLine("8) Exit");
    Console.Write("Choose: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddTask(board);
            Storage.Save(DataFile, boards);
            break;

        case "2":
            MoveTask(board);
            Storage.Save(DataFile, boards);
            break;

        case "3":
            RemoveTask(board);
            Storage.Save(DataFile, boards);
            break;

        case "4":
            board.PrintBoard();
            break;

        case "5":
            CreateBoard(boards);
            Storage.Save(DataFile, boards);
            break;

        case "6":
            currentBoardIndex = SwitchBoard(boards, currentBoardIndex);
            break;

        case "7":
            Storage.Save(DataFile, boards);
            Console.WriteLine("Saved.");
            break;

        case "8":
            Storage.Save(DataFile, boards);
            Console.WriteLine("Bye.");
            return;

        default:
            Console.WriteLine("Invalid choice.");
            break;
    }
}

static void AddTask(Board board)
{
    Console.Write("Task title: ");
    var title = (Console.ReadLine() ?? "").Trim();
    if (string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("Title cannot be empty.");
        return;
    }

    Console.Write("Assignee: ");
    var assignee = (Console.ReadLine() ?? "").Trim();
    if (string.IsNullOrWhiteSpace(assignee))
    {
        Console.WriteLine("Assignee cannot be empty.");
        return;
    }

    var priority = ReadPriority();
    var dueDate = ReadDueDateOptional();

    var newTask = board.AddTask(title, assignee, priority, dueDate);
    Console.WriteLine($"Added: {newTask}");
}

static void MoveTask(Board board)
{
    Console.Write("Task ID to move: ");
    if (!int.TryParse(Console.ReadLine(), out var moveId))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    Console.WriteLine("New Status: 1=ToDo, 2=InProgress, 3=Done");
    Console.Write("Status: ");
    if (!int.TryParse(Console.ReadLine(), out var statusNum) || statusNum is < 1 or > 3)
    {
        Console.WriteLine("Invalid status.");
        return;
    }

    var okMove = board.MoveTask(moveId, (TaskStatus)statusNum);
    Console.WriteLine(okMove ? "Task moved." : "Task not found.");
}

static void RemoveTask(Board board)
{
    Console.Write("Task ID to remove: ");
    if (!int.TryParse(Console.ReadLine(), out var removeId))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    var okRemove = board.RemoveTask(removeId);
    Console.WriteLine(okRemove ? "Task removed." : "Task not found.");
}

static void CreateBoard(List<Board> boards)
{
    Console.Write("New board name: ");
    var name = (Console.ReadLine() ?? "").Trim();

    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Board name cannot be empty.");
        return;
    }

    if (boards.Any(b => b.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
    {
        Console.WriteLine("Board already exists.");
        return;
    }

    boards.Add(new Board(name));
    Console.WriteLine($"Created board: {name}");
}

static int SwitchBoard(List<Board> boards, int currentIndex)
{
    Console.WriteLine("\nBoards:");
    for (int i = 0; i < boards.Count; i++)
    {
        var marker = i == currentIndex ? "*" : " ";
        Console.WriteLine($"{marker} {i + 1}) {boards[i].Name}");
    }

    Console.Write("Switch to #: ");
    if (!int.TryParse(Console.ReadLine(), out var num) || num < 1 || num > boards.Count)
    {
        Console.WriteLine("Invalid selection.");
        return currentIndex;
    }

    Console.WriteLine($"Switched to: {boards[num - 1].Name}");
    return num - 1;
}

static TaskPriority ReadPriority()
{
    Console.WriteLine("Priority: 1=Low, 2=Medium, 3=High");
    Console.Write("Priority: ");
    if (!int.TryParse(Console.ReadLine(), out var p) || p is < 1 or > 3)
    {
        Console.WriteLine("Invalid priority. Defaulting to Medium.");
        return TaskPriority.Medium;
    }
    return (TaskPriority)p;
}

static DateTime? ReadDueDateOptional()
{
    Console.Write("Due date (yyyy-mm-dd) or press Enter to skip: ");
    var input = (Console.ReadLine() ?? "").Trim();
    if (string.IsNullOrWhiteSpace(input)) return null;

    if (!DateTime.TryParse(input, out var due))
    {
        Console.WriteLine("Invalid date. Skipping due date.");
        return null;
    }

    return due.Date;
}
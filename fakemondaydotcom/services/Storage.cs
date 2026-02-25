using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MondayClone.Models;

namespace MondayClone.Services
{
    public static class Storage
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true
        };

        public static void Save(string filePath, List<Board> boards)
        {
            var json = JsonSerializer.Serialize(boards, Options);
            File.WriteAllText(filePath, json);
        }

        public static List<Board> Load(string filePath)
        {
            if (!File.Exists(filePath)) return new List<Board>();

            var json = File.ReadAllText(filePath);
            var boards = JsonSerializer.Deserialize<List<Board>>(json, Options);

            return boards ?? new List<Board>();
        }
    }
}
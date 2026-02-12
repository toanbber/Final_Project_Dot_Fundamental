using CsvHelper;
using Final_Project_Dot_Fundamental.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Final_Project_Dot_Fundamental.Processor
{
    internal class TodoProcessor
    {
        public void ProcessData(IEnumerable<Todo> todos)
        {
            if (todos == null || !todos.Any())
            {
                Console.WriteLine("No todo data to process.");
                return;
            }

            int totalTasks = todos.Count();
            int completedTasks = todos.Count(t => t.Completed);
            int notCompletedTasks = totalTasks - completedTasks;
            Console.WriteLine($"Total todos      : {totalTasks}");
            Console.WriteLine($"Completed        : {completedTasks}");
            Console.WriteLine($"Not completed    : {notCompletedTasks}");      
        }

        public void ExportToCsv(IEnumerable<Todo> todos, string outputPath)
        {
            if (todos == null || !todos.Any())
            {
                Console.WriteLine("No todo data to export.");
                return;
            }

            using var writer = new StreamWriter(outputPath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(todos);

            Console.WriteLine($"Exported {todos.Count()} records to {outputPath}");
        }
    }
}

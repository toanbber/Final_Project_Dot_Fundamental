using Final_Project_Dot_Fundamental.Model;
using System;
using System.Collections.Generic;
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

            int total = todos.Count();
            int completed = todos.Count(t => t.Completed);
            int notCompleted = total - completed;
            Console.WriteLine($"Total todos      : {total}");
            Console.WriteLine($"Completed        : {completed}");
            Console.WriteLine($"Not completed    : {notCompleted}");      
        }
    }
}

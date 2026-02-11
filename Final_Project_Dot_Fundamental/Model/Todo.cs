using System;
using System.Collections.Generic;
using System.Text;

namespace Final_Project_Dot_Fundamental.Model
{
    internal class Todo
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string? Title { get; set; }
        public bool Completed { get; set; }
    }
}

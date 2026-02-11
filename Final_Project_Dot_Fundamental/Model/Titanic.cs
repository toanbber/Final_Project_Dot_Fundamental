using System;
using System.Collections.Generic;
using System.Text;

namespace Final_Project_Dot_Fundamental.Model
{
    internal class Titanic
    {
        public int Pclass { get; set; }
        public string? Name { get; set; }
        public string? Sex { get; set; }
        public double? Age { get; set; }
        public int SibSp { get; set; }
        public int Parch { get; set; }
        public string? Ticket { get; set; }
        public double Fare { get; set; }
        public string? Cabin { get; set; }
        public string? Embarked { get; set; }
    }
}

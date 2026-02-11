using Final_Project_Dot_Fundamental.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Final_Project_Dot_Fundamental.Processor
{
    internal class TitanicDataProcessor
    {
        public void ProcessData(IEnumerable<Titanic> passengers)
        {
            var list = passengers.ToList();

            if (list.Count == 0)
            {
                Console.WriteLine("No data to process.");
                return;
            }
            Console.WriteLine($"Total passengers: {list.Count}");
            Console.WriteLine("\nPassengers by Class:");
            var byClass = list.GroupBy(p => p.Pclass);
            foreach (var group in byClass)
            {
                Console.WriteLine($"Class {group.Key}: {group.Count()} passengers");
            }
        }
    }
}

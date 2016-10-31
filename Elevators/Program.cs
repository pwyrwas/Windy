using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elevators.Core;
using Elevators.Core.Algorithms;

namespace Elevators
{
    class Program
    {
        static void Main(string[] args)
        {
            var building = new Building(10);

            building.AddPerson(new Person(1,    "Person1",        building.GetLevel(6),   1),     building.GetLevel(1));
            building.AddPerson(new Person(2,    "VIP1",      building.GetLevel(2),   2),     building.GetLevel(8));
            building.AddPerson(new Person(3,    "Cleaner1", building.GetLevel(4), 3), building.GetLevel(3));
            building.AddPerson(new Person(4,    "Person2", building.GetLevel(8), 1), building.GetLevel(2));
            building.AddPerson(new Person(5,    "VIP2", building.GetLevel(3), 2), building.GetLevel(9));
            building.AddPerson(new Person(6,    "Cleaner2", building.GetLevel(5), 3), building.GetLevel(4));
            
            


            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();                  // mierzenie czasu -  start pomiaru.

            List<string> commands = building.Run(new ElevatorMyHeuristicAlgorithm());      //algorytm
          //  List<string> commands = building.Run(new ElevatorFifoAlgorithmTwoElevators());      //algorytm
        
            sw.Stop();                                                                                  // mierzenie czasu -  start pomiaru.

            var ts = sw.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds,ts.Milliseconds);

            foreach (var command in commands)
            Console.WriteLine(command);

            Console.WriteLine("\n RunTime " + elapsedTime);
            Console.Write("elapsed Time: " + sw.Elapsed);

            Console.ReadKey();
        }
    }
}


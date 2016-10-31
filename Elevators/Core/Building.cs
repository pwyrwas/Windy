using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Elevators.Core.Algorithms;

namespace Elevators.Core
{
    class Building
    {
        public Elevator Elevator1;
        public Elevator Elevator2;
        public List<Level> Levels;
        public List<Person> People; 

        public Building(int numberOfLevels)
        {
            Levels = new List<Level>();
            for (int i = 0; i < numberOfLevels; i++)
                Levels.Add(new Level(i));

            Elevator1 = new Elevator(Levels[0]);
            Elevator2 = new Elevator(Levels[0]);

            People = new List<Person>();
        }

        public void ElevatorMove(Level level, Elevator elevator)
        {
            elevator.CurrentLevel = level;

        }

        public Level GetLevel(int nr)
        {
            return Levels.Find(l => l.Number == nr);
        }

        public void AddPerson(Person person, Level level)
        {
            People.Add(person);
            level.WaitingPeople.Add(person);
        }

        
        public List<string> Run(ElevatorAlgorithm elevatorAlgorithm)
        {
            return elevatorAlgorithm.Algorithm(this);
        }

        
    }
}

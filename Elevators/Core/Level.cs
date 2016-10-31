using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace Elevators.Core
{
    class Level
    {
        public int Number;
        public List<Person> WaitingPeople;
        public List<Person> FulfiledPeople;

        public Level(int number)
        {
            Number = number;
            WaitingPeople = new List<Person>();
        }
       
        public List<Person> PersonLeave(Elevator elevator)
        {
            foreach (var person in WaitingPeople)
                elevator.PersonEnter(person);
            var people = WaitingPeople.ToList();
            WaitingPeople.Clear();
            return people;
        }
        public List<Person> PersonLeaveAWithoutCleaner(Elevator elevator)
        {
            int i = 0;
            List<Person> temp2 = new List<Person>();
            foreach (var person in WaitingPeople)
            {
                if (person.Rank != 3)
                {
                    elevator.PersonEnter(person);
                    temp2.Add(person);
                    i++;
                }

            }
            foreach (var person in temp2)
            {
                WaitingPeople.Remove(person);
            }
            var people = temp2.ToList();
            
            return people;
        }
        public List<Person> PersonLeaveCleaner(Elevator elevator)
        {
            List<Person> temp = new List<Person>();
            List<Person> temp2 = new List<Person>();
            temp = WaitingPeople.ToList();
            
            int i = 0;
            foreach (var person in WaitingPeople)
            {
                if (person.Rank == 3 && i < 1)
                {
                    elevator.PersonEnter(person);
                    temp2.Add(person);
                    i++;
                }
                      
            }
            foreach (var person in temp2)
            {
                WaitingPeople.Remove(person);
            }
            var people = temp2.ToList();
            return people;
        }
        public void PersonsEnter(Elevator elevator)
        {
            var leavingPeople = elevator.People.Where(person => person.TargetLevel.Number == Number);
            elevator.People.RemoveAll(person => person.TargetLevel.Number == Number);
            FulfiledPeople.AddRange(leavingPeople);
        }
    }
}

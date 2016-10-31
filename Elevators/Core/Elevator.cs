using System.Collections.Generic;

namespace Elevators.Core
{
    class Elevator
    {
        public Level CurrentLevel;
        public List<Person> People;
        public int nr;

        public Elevator(Level currentLevel)
        {
            CurrentLevel = currentLevel;
            People = new List<Person>();
        }

        public void PersonEnter(Person person)
        {
            People.Add(person);
        }

        public void PersonLeave(Person person)
        {
            People.Remove(person);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Elevators.Core.Algorithms
{
    [System.Runtime.InteropServices.GuidAttribute("9579E233-D13E-434A-AEE4-11087909F0FE")]
    class ElevatorFifoAlgorithmOneElevator : ElevatorAlgorithm
    {
        public override List<string> Algorithm(Building building)
        {
            var commands = new List<string>();
            var existingPeople = true;
            var existingPeopleinElevator1 = false;
            List<Person> tempPeopleToElevator = new List<Person>();
            List<Person> peopleInElevator = new List<Person>();// = level.PersonLeave(building.Elevator);
            
            do          //do sprawdzania czy istnieja ludzie w windzie lub w kolejce. Głowna pętla algorytmu.
            {
                
                if (existingPeopleinElevator1 == false)
                {
                    //znajdź piętro najbliższej osoby w budynku;
                    var nearestLevel = FindNearestLevelWhereWaitingPeople(building, building.Elevator1);
                   
                    //pojdzedź do niej o ile nie jesteś na tym piętrze.
                    if (nearestLevel.Number != building.Elevator1.CurrentLevel.Number)
                    {
                        moveElevator(building, commands, nearestLevel, building.Elevator1);                             //przejazd windy na pietro
                        peopleInElevator = nearestLevel.PersonLeave(building.Elevator1);             // osoby na piętrze wsiadają
                        foreach (var person1 in peopleInElevator)
                            PersonsEnteredElevatorCommand(commands, person1, building, building.Elevator1);                       // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                    }
                }
                else     //jeśli istnieją osoby w środku windy
                {
                    var fulfiledLevel = FindNearestLevelWhereFulfilledPeople(building, peopleInElevator, building.Elevator1);           //  znajduję najbliższa osobę wśród osób w środku.
                    Level personOnRoad;// = PersonsOnTheRoadUp(building);
                    Boolean personOnRoadExist;// = personOnRoad.WaitingPeople.Count > 0;

                    if (fulfiledLevel.Number > building.Elevator1.CurrentLevel.Number)                           //Zbiera ludzli powyżej piętra na którym sie znajduje. 
                    {
                            do                                                                                  //zbieram osoby po drodze jadące do góry.
                            {
                                personOnRoad = PersonsOnTheRoadUp(building, building.Elevator1);                                    //zwracam osobę która jest najbliżej w jeździe do góry
                                personOnRoadExist = personOnRoad.WaitingPeople.Count > 0;                       //sprawdzam czy istnieją oczekujące osoby po drodzę do góry

                                    if (personOnRoadExist && (fulfiledLevel.Number != personOnRoad.Number))
                                    {
                                        getPersonOnRoad(building, personOnRoad, tempPeopleToElevator, commands, peopleInElevator, building.Elevator1);  // funkcja przesuwająca windę i zbierająca ludzi po drodze
                                    }

                            } while (personOnRoadExist && (fulfiledLevel.Number != personOnRoad.Number));  
                    }
                    else                                                                                        //Zbiera ludzli powyżej piętra na którym sie znajduje.
                    {
                            do                                                                                  //zbieram osoby po drodze jadące do góry.
                            {
                                personOnRoad = PersonsOnTheRoadDown(building, building.Elevator1);                                  //zwracam osobę która jest najbliżej w jeździe w dół
                                personOnRoadExist = personOnRoad.WaitingPeople.Count > 0;                       //sprawdzam czy istnieją oczekujące osoby po drodzę w dół
                                    if (personOnRoadExist && (fulfiledLevel.Number != personOnRoad.Number))
                                    {
                                        getPersonOnRoad(building, personOnRoad, tempPeopleToElevator, commands, peopleInElevator, building.Elevator1);  // funkcja przesuwająca windę i zbierająca ludzi po drodze
                                    }

                            } while (personOnRoadExist && (fulfiledLevel.Number != personOnRoad.Number));  
                    }

                    moveElevator(building, commands, fulfiledLevel, building.Elevator1);                             //przejazd windy na pietro
                    var personOut = WhoLeavesElevator(peopleInElevator, building.Elevator1);                   //wysiadają osoby które dojechały na piętro.
                    peopleCameOut(personOut, peopleInElevator, building, commands, building.Elevator1);              //wyjście ludzi z windy dotyczy tych co są wybrani zawsze w petli która szuka najbliższych wyjść dla pierwszej zabranej osoby.   
                    
                    tempPeopleToElevator = fulfiledLevel.PersonLeave(building.Elevator1);
                    peopleInElevator.AddRange(tempPeopleToElevator);                // osoby na piętrze wsiadają
                    foreach (var person1 in tempPeopleToElevator)
                        PersonsEnteredElevatorCommand(commands, person1, building, building.Elevator1);
                }
              
                existingPeopleinElevator1 = existPeopleInElevator(building.Elevator1);
                existingPeople = existingPeopleInBuilding(building);

            } while (existingPeople || existingPeopleinElevator1);
            




            
            return commands;
        }

        private void peopleCameOut(List<Person> personOut, List<Person> peopleInElevator, Building building, List<string> commands, Elevator elevator)    // wychodzą ludzie z windy 
        {
            foreach (var person2 in personOut)
            {
                peopleInElevator.Remove(person2);
                elevator.PersonLeave(person2);
                PersonsOutElevatorCommand(commands, person2, building, elevator);
            }
        }
        private void moveElevator(Building building, List<String> commands, Level level, Elevator elevator)
        {

            ElevatorGoesCommand(commands, building, level, building.Elevator1);                      //zapisz komende o dojechaniu do piętra.
            elevator.CurrentLevel = level;                                                           //przejedź na odpowiednie piętro.    
        }
        private void getPersonOnRoad(Building building, Level personOnRoad, List<Person> tempPeopleToElevator, List<string> commands, List<Person> peopleInElevator, Elevator elevator)
        {
            moveElevator(building, commands, personOnRoad, elevator);             //przejazd windy na piętro
            tempPeopleToElevator = personOnRoad.PersonLeave(elevator);
            peopleInElevator.AddRange(tempPeopleToElevator);            // osoby na piętrze które są na drodze i jadą do góry wsiadają.
            foreach (var person1 in tempPeopleToElevator)
                PersonsEnteredElevatorCommand(commands, person1, building, elevator);
        }
        private Boolean existPeopleInElevator(Elevator elevator)                           // zwraca wartość typu bool czy istnieje osoba w windzie
        {
            var existPeople = elevator.People.Count > 0;
            return existPeople;
        }
        private Boolean existingPeopleInBuilding(Building building)                         // zwraca wartość typu bool czy istnieje osoba w budynku
        {
            var existingPeople = building.Levels.Exists(l => l.WaitingPeople.Count > 0);
            return existingPeople;
        }
        private Level PersonsOnTheRoadUp(Building building, Elevator elevator)                                 //zwraca poziom osoby znajdującej sie na drodze do góry i jadącej do góry
        {
            int currentLevel = elevator.CurrentLevel.Number;
            var allFlorOnRoads = building.Levels.FindAll(l => l.Number > currentLevel);     //    szukam wszystich pięter ponad aktualnym piętrem windy;
            var allPersonsOnRoads = allFlorOnRoads.FindAll(l => l.WaitingPeople.Count > 0); //   szukam wszystkich osób czekających z pośród pięter ponad windą
            if (allPersonsOnRoads.Count > 0)
            {
                var personOnRoadDist = allPersonsOnRoads.First(l => l.Number > currentLevel);
                return personOnRoadDist;
            }
            return building.Elevator1.CurrentLevel;
        }
        private Level PersonsOnTheRoadDown(Building building, Elevator elevator)                               //zwraca poziom osoby znajdującej sie na drodze w dół i jadącej w dół
        {
            int currentLevel = elevator.CurrentLevel.Number;
            var allFlorOnRoads = building.Levels.FindAll(l => l.Number < currentLevel);     //    szukam wszystich pięter ponad aktualnym piętrem windy;
            var allPersonsOnRoads = allFlorOnRoads.FindAll(l => l.WaitingPeople.Count > 0); //   szukam wszystkich osób czekających z pośród pięter ponad windą
            if (allPersonsOnRoads.Count > 0)
            {
                var personOnRoadDist = allPersonsOnRoads.First(l => l.Number < currentLevel);
                return personOnRoadDist;
            }

            return building.Elevator1.CurrentLevel;
        }
        private Level FindNearestLevelWhereWaitingPeople(Building building, Elevator elevator)   // używam
        {
            int currentLevel = elevator.CurrentLevel.Number;

            int minDistance = building.Levels.Where(level => level.WaitingPeople.Count > 0).Min(level => Math.Abs(level.Number - currentLevel));
            var newLevel = building.Levels.Find(level => Math.Abs(level.Number - currentLevel) == minDistance);


            return newLevel;
        }

        private Level FindNearestLevelWhereFulfilledPeople(Building building, List<Person> newPeople, Elevator elevator)   // używam , znajdujemy tu najbliższe pietro na którym się ktoś znajduje.
        {
            int currentLevel = elevator.CurrentLevel.Number;
            var nearestLevelDist = elevator.People.Min(p => Math.Abs(p.TargetLevel.Number - currentLevel));
            var nearestLevel = elevator.People.Find(p => Math.Abs(p.TargetLevel.Number - currentLevel) == nearestLevelDist);
            var newLevel = building.Levels.Find(level => level == nearestLevel.TargetLevel);
            return newLevel;
        }

        private List<Person> WhoLeavesElevator(List<Person> persons, Elevator elevator)        //używam
        {
            var outPerson = persons.FindAll(person => person.TargetLevel.Number == elevator.CurrentLevel.Number);

            return outPerson;
        }
        // komendy wypisywane w logu.
        private void ElevatorGoesCommand(List<string> commands, Building building, Level level, Elevator elevator)
        {
            if (building.Elevator1 == elevator)
            {
                commands.Add(string.Format("Elevator 1 goes from {0} to {1}", elevator.CurrentLevel.Number, level.Number));
            }
            else if (building.Elevator2 == elevator)
            {
                commands.Add(string.Format("Elevator 2 goes from {0} to {1}", elevator.CurrentLevel.Number, level.Number));
            }

        }
        private void PersonsEnteredElevatorCommand(List<string> commands, Person person1, Building building, Elevator elevator)
        {
            if (elevator == building.Elevator1)
                commands.Add(string.Format("Person {0} entered elevator 1", person1.Name));

            if ((elevator == building.Elevator2))
                commands.Add(string.Format("Person {0} entered elevator 2", person1.Name));
        }
        private void PersonsOutElevatorCommand(List<string> commands, Person person1, Building building, Elevator elevator)
        {

            if (elevator == building.Elevator1)
                commands.Add(string.Format("Person {0} came out elevator 1", person1.Name));

            if ((elevator == building.Elevator2))
                commands.Add(string.Format("Person {0} came out elevator 2", person1.Name));
        }
    }
}

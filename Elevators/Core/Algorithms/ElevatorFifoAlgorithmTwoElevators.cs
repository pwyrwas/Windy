using System;
using System.Collections.Generic;
using System.Linq;

namespace Elevators.Core.Algorithms
{
    class ElevatorFifoAlgorithmTwoElevators : ElevatorAlgorithm
    {
        public override List<string> Algorithm(Building building)
        {
            var commands = new List<string>();
            var commands2 = new List<string>();
            var existingPeople = true;
            var existingPeopleinElevator1 = false;
            var existingPeopleinElevator2 = false;
            List<Person> tempPeopleToElevator = new List<Person>();
            List<Person> peopleInElevator1 = new List<Person>();// = level.PersonLeave(building.Elevator);
            List<Person> peopleInElevator2 = new List<Person>();

            do          //do sprawdzania czy istnieja ludzie w windzie lub w kolejce. Głowna pętla algorytmu.
            {
                // Pierwsza winda podjeżdza do najbliższej osoby
                if (existingPeopleinElevator1 == false && existingPeople)
                {
                    //znajdź piętro najbliższej osoby w budynku;
                    var nearestLevel1 = FindNearestLevelWhereWaitingPeople(building, building.Elevator1);

                    //pojdzedź do niej o ile nie jesteś na tym piętrze.
                    if (nearestLevel1.Number != building.Elevator1.CurrentLevel.Number)
                    {
                        moveElevator(building, commands, nearestLevel1, building.Elevator1);                                         //przejazd windy na pietro
                        peopleInElevator1 = nearestLevel1.PersonLeave(building.Elevator1);                                           // osoby na piętrze wsiadają
                        foreach (var person1 in peopleInElevator1)
                            PersonsEnteredElevatorCommand(commands, person1, building, building.Elevator1);                         // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                    }
                }
                // Teraz druga winda podjeżdza do najbliższej osoby
                if (existingPeopleinElevator2 == false & existingPeople)
                {
                    //znajdź piętro najbliższej osoby w budynku;
                    var nearestLevel2 = FindNearestLevelWhereWaitingPeople(building, building.Elevator2);

                    //pojdzedź do niej o ile nie jesteś na tym piętrze.
                    if (nearestLevel2.Number != building.Elevator2.CurrentLevel.Number)
                    {
                        moveElevator(building, commands2, nearestLevel2, building.Elevator2);                                         //przejazd windy na pietro
                        peopleInElevator2 = nearestLevel2.PersonLeave(building.Elevator2);                                           // osoby na piętrze wsiadają
                        foreach (var person1 in peopleInElevator2)
                            PersonsEnteredElevatorCommand(commands2, person1, building, building.Elevator2);                         // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                    }
                }

                if (existingPeopleinElevator1 != false)
                {
                    var fulfiledLevel1 = FindNearestLevelWhereFulfilledPeople(building, peopleInElevator1, building.Elevator1);           //  znajduję najbliższa osobę wśród osób w środku.
                    Level personOnRoad1;
                    Boolean personOnRoadExist1;

                    moveElevator(building, commands, fulfiledLevel1, building.Elevator1);                                   //przejazd windy na pietro
                    var personOut1 = WhoLeavesElevator(peopleInElevator1, building.Elevator1);                              //wysiadają osoby które dojechały na piętro.
                    peopleCameOut(personOut1, peopleInElevator1, building, commands, building.Elevator1);                   //wyjście ludzi z windy dotyczy tych co są wybrani zawsze w petli która szuka najbliższych wyjść dla pierwszej zabranej osoby.   

                    tempPeopleToElevator = fulfiledLevel1.PersonLeave(building.Elevator1);
                    peopleInElevator1.AddRange(tempPeopleToElevator);                                                       // osoby na piętrze wsiadają
                    foreach (var person1 in tempPeopleToElevator)
                        PersonsEnteredElevatorCommand(commands, person1, building, building.Elevator1);
                    //**************************************************************************************************************************************************//
                    if (fulfiledLevel1.Number > building.Elevator1.CurrentLevel.Number)                           //Zbiera ludzli powyżej piętra na którym sie znajduje. 
                    {
                        personOnRoad1 = PersonsOnTheRoadUp(building, building.Elevator1);                                    //zwracam osobę która jest najbliżej w jeździe do góry
                        personOnRoadExist1 = personOnRoad1.WaitingPeople.Count > 0;                       //sprawdzam czy istnieją oczekujące osoby po drodzę do góry

                        if (personOnRoadExist1 && (fulfiledLevel1.Number != personOnRoad1.Number))
                        {
                            if (personOnRoadExist1 && (fulfiledLevel1.Number != personOnRoad1.Number))
                            {
                                getPersonOnRoad(building, personOnRoad1, tempPeopleToElevator, commands, peopleInElevator1, building.Elevator1);  // funkcja przesuwająca windę i zbierająca ludzi po drodze
                            }
                        }


                    }
                    else                                                                                        //Zbiera ludzli powyżej piętra na którym sie znajduje.
                    {
                        personOnRoad1 = PersonsOnTheRoadDown(building, building.Elevator1);                                  //zwracam osobę która jest najbliżej w jeździe w dół
                        personOnRoadExist1 = personOnRoad1.WaitingPeople.Count > 0;                       //sprawdzam czy istnieją oczekujące osoby po drodzę w dół

                        if (personOnRoadExist1 && (fulfiledLevel1.Number != personOnRoad1.Number))                                                                                  //zbieram osoby po drodze jadące do góry.
                        {

                            if (personOnRoadExist1 && (fulfiledLevel1.Number != personOnRoad1.Number))
                            {
                                getPersonOnRoad(building, personOnRoad1, tempPeopleToElevator, commands, peopleInElevator1, building.Elevator1);  // funkcja przesuwająca windę i zbierająca ludzi po drodze
                            }

                        }
                    }
                    //**************************************************************************************************************************************************//    
                }
                if (existingPeopleinElevator2 != false)
                {
                    var fulfiledLevel2 = FindNearestLevelWhereFulfilledPeople(building, peopleInElevator2, building.Elevator2);           //  znajduję najbliższa osobę wśród osób w środku.
                    Level personOnRoad2;// = PersonsOnTheRoadUp(building);
                    Boolean personOnRoadExist2;// = personOnRoad.WaitingPeople.Count > 0;

                    moveElevator(building, commands2, fulfiledLevel2, building.Elevator2);                               //przejazd windy na pietro
                    var personOut2 = WhoLeavesElevator(peopleInElevator2, building.Elevator2);                           //wysiadają osoby które dojechały na piętro.
                    peopleCameOut(personOut2, peopleInElevator2, building, commands2, building.Elevator2);                //wyjście ludzi z windy dotyczy tych co są wybrani zawsze w petli która szuka najbliższych wyjść dla pierwszej zabranej osoby.   

                    tempPeopleToElevator = fulfiledLevel2.PersonLeave(building.Elevator2);
                    peopleInElevator2.AddRange(tempPeopleToElevator);                // osoby na piętrze wsiadają
                    foreach (var person1 in tempPeopleToElevator)
                        PersonsEnteredElevatorCommand(commands2, person1, building, building.Elevator2);

                    //**************************************************************************************************************************************************//
                    if (fulfiledLevel2.Number > building.Elevator2.CurrentLevel.Number)                           //Zbiera ludzli powyżej piętra na którym sie znajduje. 
                    {
                        personOnRoad2 = PersonsOnTheRoadUp(building, building.Elevator2);                                    //zwracam osobę która jest najbliżej w jeździe do góry
                        personOnRoadExist2 = personOnRoad2.WaitingPeople.Count > 0;                       //sprawdzam czy istnieją oczekujące osoby po drodzę do góry

                        if (personOnRoadExist2 && (fulfiledLevel2.Number != personOnRoad2.Number))
                        {
                            if (personOnRoadExist2 && (fulfiledLevel2.Number != personOnRoad2.Number))
                            {
                                getPersonOnRoad(building, personOnRoad2, tempPeopleToElevator, commands2, peopleInElevator2, building.Elevator2);  // funkcja przesuwająca windę i zbierająca ludzi po drodze
                            }
                        }


                    }
                    else                                                                                        //Zbiera ludzli powyżej piętra na którym sie znajduje.
                    {
                        personOnRoad2 = PersonsOnTheRoadDown(building, building.Elevator2);                                  //zwracam osobę która jest najbliżej w jeździe w dół
                        personOnRoadExist2 = personOnRoad2.WaitingPeople.Count > 0;                       //sprawdzam czy istnieją oczekujące osoby po drodzę w dół

                        if (personOnRoadExist2 && (fulfiledLevel2.Number != personOnRoad2.Number))                                                                                  //zbieram osoby po drodze jadące do góry.
                        {

                            if (personOnRoadExist2 && (fulfiledLevel2.Number != personOnRoad2.Number))
                            {
                                getPersonOnRoad(building, personOnRoad2, tempPeopleToElevator, commands2, peopleInElevator2, building.Elevator2);  // funkcja przesuwająca windę i zbierająca ludzi po drodze
                            }
                        }
                    }
                    //**************************************************************************************************************************************************// 
                }

                existingPeopleinElevator1 = existPeopleInElevator(building.Elevator1);
                existingPeopleinElevator2 = existPeopleInElevator(building.Elevator2);
                existingPeople = existingPeopleInBuilding(building);

            } while (existingPeople || existingPeopleinElevator1 || existingPeopleinElevator2);



            var sizeCommands1 = commands.Count;
            var sizeCommands2 = commands2.Count;

            commands.Add("Liczba kroków: " + sizeCommands1);
            commands2.Add("Liczba kroków: " + sizeCommands2);
            commands.Add("\n Komendy drugiej windy:\n");
            commands.AddRange(commands2);
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
            ElevatorGoesCommand(commands, building, level, elevator);                      //zapisz komende o dojechaniu do piętra.
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
            if (building.Elevator2 == elevator)
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

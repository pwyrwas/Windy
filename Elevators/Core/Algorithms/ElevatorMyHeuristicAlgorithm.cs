using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Elevators.Core.Algorithms
{
    class ElevatorMyHeuristicAlgorithm : ElevatorAlgorithm
    {
        public override List<string> Algorithm(Building building)
        {
            var commands = new List<string>();
            var commands2 = new List<string>();

            //warunki wykonywania algorytmu
            var existingPeople = true;
            var existingPeopleinElevator1 = false;
            var existingPeopleinElevator2 = false;
            List<Person> peopleInElevator1 = new List<Person>();
            List<Person> peopleInElevator2 = new List<Person>();
            List<Person> tempPeopleToElevator = new List<Person>();
            //

            // Wyszukuje osoby z listy VIP.
            var VIPList = VIPListAllVIPs(building);                            //Lista VIP, osoby z tej listy obsługiwane w pierwszej kolejności.
            var VipLvlList = FindVipLevels(building, building.Elevator1, VIPList);               //Lista poziomów na których znajdują się Vipy

            /*---------------------------------------------------------------*/

            /*---------------------------------------------------------------*/
            // Wyszukuję listę wszystkich osób sprzątajacych oraz następnie wyszkujuję listę poziomów na których znajdują się dzieci.
            var listAllCleaner = CleanerList(building);
            var LevelCleaner = findCleanerLevel(building, building.Elevator1, listAllCleaner);
            /*---------------------------------------------------------------*/

            /*---------------------------------------------------------------*/
            // Wyszukuję listę normalnych użytkowników, następnie wyszukuję listę poziomów na których Ci użytkownicy się znajdują.
            var listAllnormal = normalList(building);
            var Levelnormal = findnormalLevel(building, building.Elevator1, listAllnormal);
            /*---------------------------------------------------------------*/

            //

            do //do sprawdzania czy istnieja ludzie w windzie lub w kolejce. Głowna pętla algorytmu.
            {

                //jeśli istnieją VIP
                if (VipLvlList.Count > 0)
                {
                        #region VipExist
                    //zachowania dla windy nr. 1
                    var nearestLvlVip1 = FindNearestLevelWhereWaitingPerson(building, building.Elevator1, VipLvlList);
                    //znajduję najbliższe piętro Vipa                                               
                    //podjeżdżam po Vipa pierwszą windą,

                    if (nearestLvlVip1.Number != building.Elevator1.CurrentLevel.Number)
                    {
                        moveElevator(building, commands, nearestLvlVip1, building.Elevator1); //przejazd windy na pietro
                        existingPeopleinElevator1 = true;
                        peopleInElevator1 = nearestLvlVip1.PersonLeaveAWithoutCleaner(building.Elevator1); // osoby na piętrze wsiadają
                        VipLvlList.Remove(nearestLvlVip1);
                        foreach (var person1 in peopleInElevator1)
                        {
                            PersonsEnteredElevatorCommand(commands, person1, building, building.Elevator1);
                            // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                            VIPList.Remove(person1);
                        }
                        if (existingPeopleinElevator1 != false)
                        {
                            var fulfiledLevel1 = FindNearestLevelWhereFulfilledVip(building, peopleInElevator1, building.Elevator1); //  znajduję najbliższa osobę wśród osób w środku.

                            moveElevator(building, commands, fulfiledLevel1, building.Elevator1);
                            //przejazd windy na pietro
                            var personOut1 = WhoLeavesElevator(peopleInElevator1, building.Elevator1);
                            //wysiadają osoby które dojechały na piętro.
                            peopleCameOut(personOut1, peopleInElevator1, building, commands, building.Elevator1);
                            //wyjście ludzi z windy dotyczy tych co są wybrani zawsze w petli która szuka najbliższych wyjść dla pierwszej zabranej osoby.   
                        }
                    #endregion

                    }
                    if (VipLvlList.Count > 0)
                    {
                        #region VipExist
                        //zachowania dla windy nr. 2
                        var nearestLvlVip2 = FindNearestLevelWhereWaitingPerson(building, building.Elevator2, VipLvlList);
                        //znajduję najbliższe piętro Vipa                                               
                        //podjeżdżam po Vipa pierwszą windą,

                        if (nearestLvlVip2.Number != building.Elevator2.CurrentLevel.Number)
                        {
                            moveElevator(building, commands2, nearestLvlVip2, building.Elevator2);
                            //przejazd windy na pietro
                            existingPeopleinElevator2 = true;
                            peopleInElevator2 = nearestLvlVip2.PersonLeaveAWithoutCleaner(building.Elevator2);
                            // osoby na piętrze wsiadają
                            VipLvlList.Remove(nearestLvlVip2);
                            foreach (var person1 in peopleInElevator2)
                            {
                                PersonsEnteredElevatorCommand(commands2, person1, building, building.Elevator2);
                                // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                                VIPList.Remove(person1);
                            }
                            if (existingPeopleinElevator2 != false)
                            {
                                var fulfiledLevel2 = FindNearestLevelWhereFulfilledVip(building, peopleInElevator2, building.Elevator2); //  znajduję najbliższa osobę wśród osób w środku.

                                moveElevator(building, commands2, fulfiledLevel2, building.Elevator2);
                                //przejazd windy na pietro
                                var personOut2 = WhoLeavesElevator(peopleInElevator2, building.Elevator2);
                                //wysiadają osoby które dojechały na piętro.
                                peopleCameOut(personOut2, peopleInElevator2, building, commands2, building.Elevator2);
                                //wyjście ludzi z windy dotyczy tych co są wybrani zawsze w petli która szuka najbliższych wyjść dla pierwszej zabranej osoby.   
                            }
                        }
                        #endregion
                    }
                    // 
                } //jesli VIP już rozwiezione lub nie było.
                else if ((building.Elevator1.People.Count < 1 || building.Elevator2.People.Count < 1) && listAllCleaner.Count > 0)                                    //wykonuję jeżeli w windzie 1 lub drguiej nie ma żadnej osoby oraz stan listy osób sprzątających jest dodatni.
                {
                    #region obsługaCeanerów
                    if (building.Elevator1.People.Count < 1 && LevelCleaner.Count > 0)
                    {
                        var nearestLvlCleaner1 = FindNearestLevelWhereWaitingPerson(building, building.Elevator1, LevelCleaner);               //znajduję najbliższe piętro osoby sprzątającej  

                        if (nearestLvlCleaner1.Number != building.Elevator1.CurrentLevel.Number)
                        {
                            moveElevator(building, commands, nearestLvlCleaner1, building.Elevator1);
                            //przejazd windy na pietro
                            existingPeopleinElevator1 = true;
                            peopleInElevator1 = nearestLvlCleaner1.PersonLeaveCleaner(building.Elevator1); // osoby na piętrze wsiadają
                            if(peopleInElevator1.Count > 0)
                            LevelCleaner.Remove(nearestLvlCleaner1);

                            foreach (var person1 in peopleInElevator1)
                            {
                                if (person1.Rank == 3)
                                {
                                    PersonsEnteredElevatorCommand(commands, person1, building, building.Elevator1);
                                    // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                                    listAllCleaner.Remove(person1);
                                }
                            }
                        }
                        else
                        {
                            existingPeopleinElevator1 = true;
                            peopleInElevator1 = nearestLvlCleaner1.PersonLeaveCleaner(building.Elevator1); // osoby na piętrze wsiadają
                            if (peopleInElevator1.Count > 0)
                                LevelCleaner.Remove(nearestLvlCleaner1);

                            foreach (var person1 in peopleInElevator1)
                            {
                                if (person1.Rank == 3)
                                {
                                    PersonsEnteredElevatorCommand(commands, person1, building, building.Elevator1);
                                    // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                                    listAllCleaner.Remove(person1);
                                }
                            }
                        }
                        
                        
                        if (existingPeopleinElevator1 != false)
                        {
                            var fulfiledLevel1 = FindNearestLevelWhereFulfilledCleaner(building, peopleInElevator1, building.Elevator1); //  znajduję najbliższa osobę wśród osób w środku.

                            moveElevator(building, commands, fulfiledLevel1, building.Elevator1);
                            //przejazd windy na pietro
                            var personOut1 = WhoLeavesElevator(peopleInElevator1, building.Elevator1);
                            //wysiadają osoby które dojechały na piętro.
                            peopleCameOut(personOut1, peopleInElevator1, building, commands, building.Elevator1);
                            //wyjście ludzi z windy dotyczy tych co są wybrani zawsze w petli która szuka najbliższych wyjść dla pierwszej zabranej osoby.   
                        }

                    }
                    else if (building.Elevator2.People.Count < 1 && LevelCleaner.Count > 0)
                    {
                        var nearestLvlCleaner2 = FindNearestLevelWhereWaitingPerson(building, building.Elevator2, LevelCleaner);               //znajduję najbliższe piętro osoby sprzątającej     

                        if (nearestLvlCleaner2.Number != building.Elevator2.CurrentLevel.Number)
                        {
                            moveElevator(building, commands2, nearestLvlCleaner2, building.Elevator2);
                            //przejazd windy na pietro
                            existingPeopleinElevator2 = true;
                            peopleInElevator2 = nearestLvlCleaner2.PersonLeaveCleaner(building.Elevator2);
                                // osoby na piętrze wsiadają
                            LevelCleaner.Remove(nearestLvlCleaner2);

                            foreach (var person1 in peopleInElevator2)
                            {
                                if (person1.Rank == 3)
                                {
                                    PersonsEnteredElevatorCommand(commands2, person1, building, building.Elevator2);
                                    // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                                    listAllCleaner.Remove(person1);
                                }
                            }
                        }
                        else
                        {
                            existingPeopleinElevator2 = true;
                            peopleInElevator2 = nearestLvlCleaner2.PersonLeaveCleaner(building.Elevator2);
                            // osoby na piętrze wsiadają
                            LevelCleaner.Remove(nearestLvlCleaner2);

                            foreach (var person1 in peopleInElevator2)
                            {
                                if (person1.Rank == 3)
                                {
                                    PersonsEnteredElevatorCommand(commands2, person1, building, building.Elevator2);
                                    // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                                    listAllCleaner.Remove(person1);
                                }
                            }
                        }
                        
                        if (existingPeopleinElevator2 != false)
                        {
                            var fulfiledLevel2 = FindNearestLevelWhereFulfilledCleaner(building, peopleInElevator2, building.Elevator2); //  znajduję najbliższa osobę wśród osób w środku.

                            moveElevator(building, commands2, fulfiledLevel2, building.Elevator2);
                            //przejazd windy na pietro
                            var personOut2 = WhoLeavesElevator(peopleInElevator2, building.Elevator2);
                            //wysiadają osoby które dojechały na piętro.
                            peopleCameOut(personOut2, peopleInElevator2, building, commands2, building.Elevator2);
                            //wyjście ludzi z windy dotyczy tych co są wybrani zawsze w petli która szuka najbliższych wyjść dla pierwszej zabranej osoby.   
                        }
                    }


                    #endregion
                }
                else if (existingPeopleinElevator1 || existingPeopleinElevator2 || existingPeople)
                {
                    
                    // Pierwsza winda podjeżdza do najbliższej osoby
                    if (existingPeopleinElevator1 == false && existingPeople)
                    {
                        //znajdź piętro najbliższej osoby w budynku;
                        var nearestLevel1 = FindNearestLevelWhereWaitingPerson(building, building.Elevator1, Levelnormal);

                        //pojdzedź do niej o ile nie jesteś na tym piętrze.
                        if (nearestLevel1.Number != building.Elevator1.CurrentLevel.Number)
                        {
                            moveElevator(building, commands, nearestLevel1, building.Elevator1);
                                //przejazd windy na pietro
                            peopleInElevator1 = nearestLevel1.PersonLeave(building.Elevator1);
                                // osoby na piętrze wsiadają
                            foreach (var person1 in peopleInElevator1)
                            {
                                PersonsEnteredElevatorCommand(commands, person1, building, building.Elevator1);
                                // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                                Levelnormal.Remove(nearestLevel1);
                            }
                        }
                        existingPeople = existingPeopleInBuilding(building);
                    }
                    // Teraz druga winda podjeżdza do najbliższej osoby
                    if (existingPeopleinElevator2 == false & existingPeople)
                    {
                        //znajdź piętro najbliższej osoby w budynku;
                        var nearestLevel2 = FindNearestLevelWhereWaitingPerson(building, building.Elevator2, Levelnormal);

                        //pojdzedź do niej o ile nie jesteś na tym piętrze.
                        if (nearestLevel2.Number != building.Elevator2.CurrentLevel.Number)
                        {
                            moveElevator(building, commands2, nearestLevel2, building.Elevator2);                                         //przejazd windy na pietro
                            peopleInElevator2 = nearestLevel2.PersonLeave(building.Elevator2);                                           // osoby na piętrze wsiadają
                            foreach (var person1 in peopleInElevator2)
                            {   PersonsEnteredElevatorCommand(commands2, person1, building, building.Elevator2);                         // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                                Levelnormal.Remove(nearestLevel2);
                            }
                              
                                
                        }
                        existingPeople = existingPeopleInBuilding(building);
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
                        {
                            PersonsEnteredElevatorCommand(commands, person1, building, building.Elevator2);                         // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                            Levelnormal.Remove(fulfiledLevel1);
                        }
                        //**************************************************************************************************************************************************//
                        if (fulfiledLevel1.Number > building.Elevator1.CurrentLevel.Number)                           //Zbiera ludzli powyżej piętra na którym sie znajduje. 
                        {
                            personOnRoad1 = PersonsOnTheRoadUp(building, building.Elevator1);                                    //zwracam osobę która jest najbliżej w jeździe do góry
                            personOnRoadExist1 = personOnRoad1.WaitingPeople.Count > 0;                       //sprawdzam czy istnieją oczekujące osoby po drodzę do góry

                            if (personOnRoadExist1 && (fulfiledLevel1.Number != personOnRoad1.Number))
                            {
                                if (personOnRoadExist1 && (fulfiledLevel1.Number != personOnRoad1.Number))
                                {
                                    getPersonOnRoad(building, personOnRoad1, tempPeopleToElevator, commands, peopleInElevator1, building.Elevator1, Levelnormal);  // funkcja przesuwająca windę i zbierająca ludzi po drodze
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
                                    getPersonOnRoad(building, personOnRoad1, tempPeopleToElevator, commands, peopleInElevator1, building.Elevator1, Levelnormal);  // funkcja przesuwająca windę i zbierająca ludzi po drodze
                                }
                            }
                        }
                        //**************************************************************************************************************************************************//    
                    }
                    if (existingPeopleinElevator2 != false)
                    {
                        var fulfiledLevel2 = FindNearestLevelWhereFulfilledPeople(building, peopleInElevator2, building.Elevator2);             //  znajduję najbliższa osobę wśród osób w środku.
                        Level personOnRoad2;
                        Boolean personOnRoadExist2;

                        moveElevator(building, commands2, fulfiledLevel2, building.Elevator2);                                                  //przejazd windy na pietro
                        var personOut2 = WhoLeavesElevator(peopleInElevator2, building.Elevator2);                                              //wysiadają osoby które dojechały na piętro.
                        peopleCameOut(personOut2, peopleInElevator2, building, commands2, building.Elevator2);                                  //wyjście ludzi z windy dotyczy tych co są wybrani zawsze w petli która szuka najbliższych wyjść dla pierwszej zabranej osoby.   

                        tempPeopleToElevator = fulfiledLevel2.PersonLeave(building.Elevator2);
                        peopleInElevator2.AddRange(tempPeopleToElevator);                                                                       // osoby na piętrze wsiadają
                        foreach (var person1 in tempPeopleToElevator)
                        {
                            PersonsEnteredElevatorCommand(commands2, person1, building, building.Elevator2);                         // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                            Levelnormal.Remove(fulfiledLevel2);
                        }
                        #region UpOrDown
                        //**************************************************************************************************************************************************//
                        if (fulfiledLevel2.Number > building.Elevator2.CurrentLevel.Number)                           //Zbiera ludzli powyżej piętra na którym sie znajduje. 
                        {
                            personOnRoad2 = PersonsOnTheRoadUp(building, building.Elevator2);                                    //zwracam osobę która jest najbliżej w jeździe do góry
                            personOnRoadExist2 = personOnRoad2.WaitingPeople.Count > 0;                       //sprawdzam czy istnieją oczekujące osoby po drodzę do góry

                            if (personOnRoadExist2 && (fulfiledLevel2.Number != personOnRoad2.Number))
                            {
                                if (personOnRoadExist2 && (fulfiledLevel2.Number != personOnRoad2.Number))
                                {
                                    getPersonOnRoad(building, personOnRoad2, tempPeopleToElevator, commands2, peopleInElevator2, building.Elevator2, Levelnormal);  // funkcja przesuwająca windę i zbierająca ludzi po drodze
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
                                    getPersonOnRoad(building, personOnRoad2, tempPeopleToElevator, commands2, peopleInElevator2, building.Elevator2, Levelnormal);  // funkcja przesuwająca windę i zbierająca ludzi po drodze
                                }
                            }
                        }
                        #endregion
                        //**************************************************************************************************************************************************// 
                    }
                }//kod ogólny - koniec
                    

                existingPeopleinElevator1 = existPeopleInElevator(building.Elevator1);
                existingPeopleinElevator2 = existPeopleInElevator(building.Elevator2);
                existingPeople = existingPeopleInBuilding(building);
            } while (existingPeople || existingPeopleinElevator1 || existingPeopleinElevator2);
            #region informacjeKońcowe
            var sizeCommands1 = commands.Count;
            var sizeCommands2 = commands2.Count;

            commands.Add("Liczba kroków: " + sizeCommands1);
            commands2.Add("Liczba kroków: " + sizeCommands2);
            commands.Add("\n Komendy drugiej windy:\n");
            commands.AddRange(commands2);

            return commands;
            #endregion
        }
        /****************************************************************B******************************************************************************/
        private void getPersonOnRoad(Building building, Level personOnRoad, List<Person> tempPeopleToElevator, List<string> commands, List<Person> peopleInElevator, Elevator elevator, List<Level> Levelnormal)
        {
            moveElevator(building, commands, personOnRoad, elevator);                                               //przejazd windy na piętro
            tempPeopleToElevator = personOnRoad.PersonLeave(elevator);
            peopleInElevator.AddRange(tempPeopleToElevator);                                                        // osoby na piętrze które są na drodze i jadą do góry wsiadają.
            foreach (var person1 in tempPeopleToElevator)
                PersonsEnteredElevatorCommand(commands, person1, building, elevator);
            foreach (var person1 in tempPeopleToElevator)
            {
                PersonsEnteredElevatorCommand(commands, person1, building, building.Elevator2);                         // wszystkie osoby bo przeszukuję po wszystkich osobach na piętrze.
                Levelnormal.Remove(personOnRoad);
            }
                              
        }
        private List<Person> KidsList(Building building)                                                            //wyszukuje dzieci
        {
            var AllKids = building.People.FindAll(p => p.Rank == 4);
            return AllKids;
        }
        private List<Level> findKidsLevel(Building building, Elevator elevator, List<Person> Kids)                 //wyszukuję piętra na których znajdują się dzieci na podstawie listy  z dziećmi.
        {
            List<Level> KidsLevels = new List<Level>();

            foreach (var level in building.Levels)
                foreach (var person in Kids)
                    if (!KidsLevels.Contains(level) && level.WaitingPeople.Contains(person))
                        KidsLevels.Add(level);
            return KidsLevels;
        }
        private List<Person> normalList(Building building)                                                          //wyszukuje normalne osoby
        {
            var Allnormal = building.People.FindAll(p => p.Rank == 1);
            return Allnormal;
        }
        private List<Level> findnormalLevel(Building building, Elevator elevator, List<Person> normal)              //wyszukuję piętra na których znajdują się normalne osoby na podstawie listy  z normalnymi presonami.
        {
            List<Level> normalLevels = new List<Level>();

            foreach (var level in building.Levels)
                foreach (var person in normal)
                    if (!normalLevels.Contains(level) && level.WaitingPeople.Contains(person))
                        normalLevels.Add(level);
            return normalLevels;
        }
        private List<Person> CleanerList(Building building)                                                         //wyszukuje osoby sprzątajace.
        {
            var AllCleaner = building.People.FindAll(p => p.Rank == 3);
            return AllCleaner;
        }
        private List<Level> findCleanerLevel(Building building, Elevator elevator, List<Person> Cleaner)            //wyszukuję piętra na których znajdują się osoby sprzątające na podstawie listy  z osobami sprzątającymi.
        {
            List<Level> CleanerLevels = new List<Level>();

            foreach (var level in building.Levels)
                foreach (var person in Cleaner)
                    if (!CleanerLevels.Contains(level) && level.WaitingPeople.Contains(person))
                        CleanerLevels.Add(level);
            return CleanerLevels;
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
            ElevatorGoesCommand(commands, building, level, elevator);                                               //zapisz komende o dojechaniu do piętra.
            elevator.CurrentLevel = level;                                                                          //przejedź na odpowiednie piętro.    
        }
        private Boolean existPeopleInElevator(Elevator elevator)                                                    // zwraca wartość typu bool czy istnieje osoba w windzie
        {
            var existPeople = elevator.People.Count > 0;
            return existPeople;
        }
        private Boolean existingPeopleInBuilding(Building building)                                                 // zwraca wartość typu bool czy istnieje osoba w budynku
        {
            var existingPeople = building.Levels.Exists(l => l.WaitingPeople.Count > 0);
            return existingPeople;
        }
        private List<Person> VIPListAllVIPs(Building building)                                                      //wyszukuje ludzi VIP
        {
            var AllVip = building.People.FindAll(p => p.Rank == 2);
            return AllVip;
        }
        private Level FindNearestLevelWhereWaitingPeople(Building building, Elevator elevator)                      //najbliższe piętro gdzie znajduje się osoba czekająca
        {
            int currentLevel = elevator.CurrentLevel.Number;

            int minDistance = building.Levels.Where(level => level.WaitingPeople.Count > 0).Min(level => Math.Abs(level.Number - currentLevel));
            var newLevel = building.Levels.Find(level => Math.Abs(level.Number - currentLevel) == minDistance);

            return newLevel;
        }
        private Level FindNearestLevelWhereWaitingPerson(Building building, Elevator elevator, List<Level> Vip)        // najbliższe piętro gdzie znajduje się VIP
        {
            int currentLevel = elevator.CurrentLevel.Number;

            int minDistance = Vip.Where(level => level.WaitingPeople.Count > 0).Min(level => Math.Abs(level.Number - currentLevel));
            var newLevel = Vip.Find(level => Math.Abs(level.Number - currentLevel) == minDistance);

            return newLevel;
        }
        private List<Person> WhoLeavesElevator(List<Person> persons, Elevator elevator)                             //lista osóbopuszczających piętro
        {
            var outPerson = persons.FindAll(person => person.TargetLevel.Number == elevator.CurrentLevel.Number);

            return outPerson;
        }
        private Level FindNearestLevelWhereFulfilledVip(Building building, List<Person> newPeople, Elevator elevator)   // używam , znajdujemy tu najbliższe pietro na którym się ktoś znajduje.
        {
            int currentLevel = elevator.CurrentLevel.Number;
            var nearestLevelDist = elevator.People.Min(p => Math.Abs(p.TargetLevel.Number - currentLevel));
            var nearestLevel = elevator.People.Find(p => Math.Abs(p.TargetLevel.Number - currentLevel) == nearestLevelDist && p.Rank == 2);
            var newLevel = building.Levels.Find(level => level == nearestLevel.TargetLevel);
            return newLevel;
        }
        private Level FindNearestLevelWhereFulfilledCleaner(Building building, List<Person> newPeople, Elevator elevator)   // używam , znajdujemy tu najbliższe pietro na którym się ktoś znajduje.
        {
            int currentLevel = elevator.CurrentLevel.Number;
            var nearestLevelDist = elevator.People.Min(p => Math.Abs(p.TargetLevel.Number - currentLevel));
            var nearestLevel = elevator.People.Find(p => Math.Abs(p.TargetLevel.Number - currentLevel) == nearestLevelDist && p.Rank == 3);
            var newLevel = building.Levels.Find(level => level == nearestLevel.TargetLevel);
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
        private List<Level> FindVipLevels(Building building, Elevator elevator, List<Person> VIP)                          //funkcja służąca do odnajdywania pięter na któych znajdują się Vipy
        {
            List<Level> VipLevels = new List<Level>();

            foreach (var level in building.Levels)
                foreach (var person in VIP)
                    if (!VipLevels.Contains(level) && level.WaitingPeople.Contains(person))
                        VipLevels.Add(level);
            return VipLevels;
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

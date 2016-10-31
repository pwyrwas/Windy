using System;

namespace Elevators.Core
{
    class Person
    {
        public int Id;
        public string Name;
        public Level TargetLevel;
        public int Rank;                //ranga 1 - zwykła osoba, 2 - vip, 3 - pracownik sprzątający.

        //  Zwykła osoba nie ma żadnych wymagań co do jazdy.
        //  Vip jeździ bezpośrednio na swoje piętro. Może jechać z kimś ale ma pierwszeństwo.
        //  Pracownik sprzatający ma swoj bagaż i jeździ sam. Co do priorytetu jeździ tak jak zwykła osoba tyle że winda musi być pusta.

        public Person(int id, string name, Level targetLevel,int rank)
        {
            Id = id;
            Name = name;
            Rank = rank;
            TargetLevel = targetLevel;
        }
    }
}

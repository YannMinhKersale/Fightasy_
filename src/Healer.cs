using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fightasy
{
    class Healer : Character
    {
        bool usedHeal;

        public Healer()
        {
            name = "Healer";
            classColor = ConsoleColor.White;
            health = 4;
            damage = 1;
            capacityName = "Soins";
            usedHeal = false;
            description = "Récupère un point de vie (+♥).";
        }
        public override void SpecialCapacity() { ++health; }
    }
}

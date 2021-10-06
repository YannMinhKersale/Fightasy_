using System;

namespace Fightasy
{
    class Damager : Character
    {
        public Damager()
        {
            name = "Damager";
            classColor = ConsoleColor.Red;
            health = 3;
            damage = 2;
            capacityName = "Rage";
            description = "Inflige en retour les dégâts qui lui sont infligés durant ce tour. Les dégâts sont quand même subis par le Damager.";
        }
    }
}

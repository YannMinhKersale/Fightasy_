using System;

namespace Fightasy
{
    class Wizard : Character
    {
        public Wizard()
        {
            name = "Wizard";
            classColor = ConsoleColor.Blue;
            health = 3;
            damage = 3;
            capacityName = "Contre-sort";
            description = "Bloque la capacité spéciale de l'adversaire si elle est lancée, et inflige un point de dégât en retour.";
        }
    }
}

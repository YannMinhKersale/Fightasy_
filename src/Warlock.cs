using System;

namespace Fightasy
{
    class Warlock : Character
    {
        bool alreadyGave = false;
        public Warlock()
        {
            name = "Warlock";
            classColor = ConsoleColor.Magenta;
            health = 5;
            damage = 1;
            capacityName = "Don de sang";
            description = "Invoque les dieux pour gagner de la force. !Attention! leur réponse dépend de leur humeur";

        }
        public override void SpecialCapacity()
        {
            if (!alreadyGave)
            {
                Random rand = new();
                int prob = rand.Next(10);
                if (prob == 0) health = 0;
                else if (prob <= 4) --health;
                else if (prob <= 8) ++damage;
                else if (prob == 9) damage += 4;
            }
            else alreadyGave = true;
        }
    }
}

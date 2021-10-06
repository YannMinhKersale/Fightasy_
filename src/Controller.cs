using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fightasy
{
    class Controller
    {
        /** Classe du joueur humain. */
        public Character player;
        /** Code du choix de l'action du joueur. */
        public int playerAction;
        /** Classe du joueur IA. */
        public Character computer;
        /** Code du choix de l'action de l'IA. */
        public int computerAction;

        Random rand;

        /** Liste des classes disponibles, une par joueurs. */
        public List<Character> playerCharacters = new List<Character> { new Damager(), new Tank(), new Healer(), new Warlock(), new Wizard() };
        public List<Character> iaCharacters = new List<Character> { new Damager(), new Tank(), new Healer(), new Warlock(), new Wizard() };

        /** Référence à la classe d'affichage (Human-Machine Interface Console User Interface)*/
        HMICUI display;

        int gamemode;
        int difficulty;

        /** Tableau de scores sur les simulations. */
        int[,] scores = new int[5, 5];

        public Controller()
        {
            display = new HMICUI(this);
            rand = new();

            // Choix du mode de jeu.
            gamemode = display.ChooseBox(0) - 1;

            // Dans le cas d'une Simulation
            if(gamemode == 1)
            {
                difficulty = 3;
                display.DisplayTextBox(new string[1] { "La simulation commence" }, true);
            }
        }

        /** Méthode contenant tout le cycle du jeu. */
        public void Game()
        {
            // Dans le cas d'une partie classique.
            if (gamemode == 0)
            {
                difficulty = display.ChooseBox(1) - 1;

                playerCharacters = new List<Character> { new Damager(), new Tank(), new Healer(), new Warlock(), new Wizard() };
                iaCharacters = new List<Character> { new Damager(), new Tank(), new Healer(), new Warlock(), new Wizard() };
                // Choix des classes pour les deux joueurs.
                player = playerCharacters[display.ChooseBox(2) - 1];
                computer = iaCharacters[rand.Next(iaCharacters.Count - 1)];

                // Affichage du titre du jeu.
                display.DisplayTextBox(new string[1] { "FIGHTASY : le jeu de combat" }, true);

                // Affichage du résultat des choix.
                display.DisplayTextBox(new string[2] { $" Vous avez choisi la classe {player.GetName()}"
                                         , $"L'IA a choisi de prendre un {computer.GetName()} !" }, true);

                System.Threading.Thread.Sleep(2000);
                Console.Clear();

                display.DisplayTextBox(new string[1] { "FIGHTASY : le jeu de combat" }, true);
                display.DisplayTextBox(new string[1] { " Que le combat commence !" }, true);

                System.Threading.Thread.Sleep(1000);
                Console.Clear();

                // Boucle de gameplay d'une partie.
                while (!player.isDead() && !computer.isDead())
                {
                    // Choix de l'action du joueur.
                    playerAction = display.ChooseBox(3);
                    // Choix de l'action de l'Intelligence artificielle.
                    computerAction = IAChoices();

                    // Concaténation des codes de choix des deux joueurs en un seul code.
                    string resultAction = String.Concat(playerAction.ToString(), computerAction.ToString());
                    // Affichage du faux écran en fonction du code resultAction.
                    display.DisplayScreen(resultAction);

                    // Variables temporaires des information sur le résultat du tour.
                    string[] turnStateMessage = new string[] { "" };
                    string[] specialMessage = new string[] { "" };
                    // Variables temporaires pour connaître les changements de stats des joueurs.

                    // Interprétation du code représentant les actions du tour.
                    switch (resultAction)
                    {
                        //Joueur : Défend |-| IA : Défend.
                        case "22": turnStateMessage = new string[] { "Les deux joueurs se défendent" }; break;
                        //Joueur : Attaque |-| IA : Défend.
                        case "12": turnStateMessage = new string[] { "Vous attaquez, mais votre adversaire se défend" }; break;
                        //Joueur : Défend |-| IA : Attaque
                        case "21": turnStateMessage = new string[] { "Vous vous défendez face à l'attaque ennemie" }; break;
                        // Joueur : Attaque |-| IA : Attaque.
                        case "11": { 
                            // Les deux joeurs s'infligent leurs dégâts.
                            computer.Hit(player.GetDamage());
                            player.Hit(computer.GetDamage());

                            //Affichage des messages d'état du tour.
                            turnStateMessage = new string[] { "Les deux joueurs s'infligent tous deux des dégâts"};
                            break;
                        }
                        //Joueur : Spécial |-| IA : Spécial 
                        case "33": {
                            turnStateMessage = new string[] { $"Vous utilisez {player.GetCapacityName()} contre {computer.GetCapacityName()}" };

                            if (player.GetName() == "Warlock") specialMessage = new string[2] { WarlockSpecial(true)[0], "" };
                            else if (player.GetName() == "Tank") TankSpecial(true);
                            else player.SpecialCapacity();

                            if (computer.GetName() == "Warlock") specialMessage[1] += WarlockSpecial(false)[0];
                            else if (computer.GetName() == "Tank") TankSpecial(false);
                            else computer.SpecialCapacity();

                            break;
                        }
                        //Joueur : Attaque |-| IA : Spécial
                        case "13": {
                            turnStateMessage = new string[] { $"Vous attaquez contre {computer.GetCapacityName()}" };

                            if (computer.GetName() == "Warlock") specialMessage = WarlockSpecial(false);
                            else if (computer.GetName() == "Tank") TankSpecial(false);
                            else computer.SpecialCapacity();

                            computer.Hit(player.GetDamage());
                            if (computer.GetName() == "Damager") player.Hit(player.GetDamage());

                            break;
                        }
                        //Joueur : Spécial |-| IA : Attaque
                        case "31": {
                            turnStateMessage = new string[] { $"Vous utilisez {player.GetCapacityName()} contre l'attaque ennemie" };

                            if (player.GetName() == "Warlock") specialMessage = WarlockSpecial(true);
                            else if (player.GetName() == "Tank") TankSpecial(true);
                            else player.SpecialCapacity();

                            player.Hit(computer.GetDamage());
                            if (player.GetName() == "Damager") computer.Hit(computer.GetDamage());

                            break;
                        }
                        //Joueur : Défend |-| IA : Spécial
                        case "23": {
                            turnStateMessage = new string[] { $"Vous essayez de vous défendre contre {computer.GetCapacityName()}" };

                            if (computer.GetName() == "Warlock") specialMessage = WarlockSpecial(false);
                            else if (computer.GetName() == "Tank") TankSpecial(false);
                            else computer.SpecialCapacity();

                            break;
                        }
                        //Joueur : Spécial |-| IA : Défend
                        case "32": {
                            turnStateMessage = new string[] { $"Vous utilisez {player.GetCapacityName()} contre la défense ennemie" };

                            if (player.GetName() == "Warlock") specialMessage = WarlockSpecial(true);
                            else if (player.GetName() == "Tank") TankSpecial(true);
                            else player.SpecialCapacity();

                            break;
                        }
                    }

                    // Affichage des effets de couleurs sur l'écran.
                    display.ColoredScreen(resultAction);
                    // Affichage du message résumant les actions du tour.
                    display.DisplayTextBox(turnStateMessage, false);
                    System.Threading.Thread.Sleep(2000);

                    // Affichage d'un message spécial.
                    if (specialMessage[0] != "")
                    {
                        display.DisplayTextBox(specialMessage, true);
                        System.Threading.Thread.Sleep(2000);
                    }
                }

                int gameResult;
                // Egalité
                if (computer.GetHealth() == 0 && player.GetHealth() == 0) gameResult = 1;
                // Victoire Joueur
                else if (computer.GetHealth() == 0) gameResult = 0;
                // Défaite joueur
                else gameResult = 2;

                // Envoi du résultat de la partie à l'IHM.
                display.DisplayEndScreen(gameResult);
                // Affichage de l'écran de fin et choix de l'utilisateur pour rejouer ou non.
                if (display.ChooseBox(4) == 1) Game();
                else return;
            }
            // Dans le mode simulation.
            else
            {
                // On calcule le pourcentage de victoire à l'envers pour toutes les cases du tableau horizontal.
                for (int j = 0; j < playerCharacters.Count; j++) for (int k = j; k < playerCharacters.Count; k++) scores[k, j] = 1000;

                // Sur 100 simulations.
                for (int a = 0; a < 100; a++)
                {
                    // Pour chaque personnage de la première liste.
                    for (int j = 0; j < playerCharacters.Count; j++)
                    {
                        // On choisit le personnage de la deuxième liste où
                        for (int k = j; k < 5; k++)
                        {
                            // On recrée les listes de classes à chaque simulation
                            playerCharacters = new List<Character> { new Damager(), new Tank(), new Healer(), new Warlock() };
                            iaCharacters = new List<Character> { new Damager(), new Tank(), new Healer(), new Warlock(), new Wizard() };

                            // Si la case du tableau n'est pas une case diagonale centrale.
                            if (j != k)
                            {
                                // On assigne à chaque IA un personnage dans les listes.
                                player = playerCharacters[j];
                                computer = iaCharacters[k];

                                // Boucle des actions d'une simulation qui ne dépasse pas 50 tours.
                                int nbTurn = 0;
                                while (!player.isDead() && !computer.isDead() && nbTurn < 50)
                                {
                                    // Actions de chaque IA choisie au hasard.
                                    computerAction = rand.Next(1, 4);
                                    playerAction = rand.Next(1, 4);

                                    // Concaténation du code de chacun des choix.
                                    string resultAction = String.Concat(playerAction.ToString(), computerAction.ToString());

                                    // En fonction de ce code :
                                    switch (resultAction)
                                    {
                                        // Joueur : Attaque |-| IA : Attaque
                                        case "11":
                                            {
                                                computer.Hit(player.GetDamage());
                                                player.Hit(computer.GetDamage());
                                                break;
                                            }
                                        //Joueur : Spécial |-| IA : Spécial 
                                        case "33":
                                            {
                                                if (player.GetName() == "Warlock") WarlockSpecial(true);
                                                else if (player.GetName() == "Tank") TankSpecial(true);
                                                else player.SpecialCapacity();

                                                if (computer.GetName() == "Warlock") WarlockSpecial(false);
                                                else if (computer.GetName() == "Tank") TankSpecial(false);
                                                else computer.SpecialCapacity();

                                                break;
                                            }
                                        //Joueur : Attaque |-| IA : Spécial
                                        case "13":
                                            {
                                                if (computer.GetName() == "Warlock") WarlockSpecial(false);
                                                else if (computer.GetName() == "Tank") TankSpecial(false);
                                                else computer.SpecialCapacity();

                                                computer.Hit(player.GetDamage());
                                                if (computer.GetName() == "Damager") player.Hit(player.GetDamage());

                                                break;
                                            }
                                        //Joueur : Spécial |-| IA : Attaque
                                        case "31":
                                            {
                                                if (player.GetName() == "Warlock") WarlockSpecial(true);
                                                else if (player.GetName() == "Tank") TankSpecial(true);
                                                else player.SpecialCapacity();

                                                player.Hit(computer.GetDamage());
                                                if (player.GetName() == "Damager") computer.Hit(computer.GetDamage());

                                                break;
                                            }
                                        //Joueur : Défend |-| IA : Spécial
                                        case "23":
                                            {
                                                if (computer.GetName() == "Warlock") WarlockSpecial(false);
                                                else if (computer.GetName() == "Tank") TankSpecial(false);
                                                else computer.SpecialCapacity();

                                                break;
                                            }
                                        //Joueur : Spécial |-| IA : Défend
                                        case "32":
                                            {
                                                if (player.GetName() == "Warlock") WarlockSpecial(true);
                                                else if (player.GetName() == "Tank") TankSpecial(true);
                                                else player.SpecialCapacity();

                                                break;
                                            }
                                    }
                                    // Léger affichage de chaque tours.
                                    display.DisplayTextBox(new string[2] { $"Joueur : {player.GetName()}, {player.GetHealth()}", $"IA : {computer.GetName()}, {computer.GetHealth()}" }, true);
                                    nbTurn += 1;
                                }

                                // Attribution des points du gagnant et du perdant dans le tableau.
                                if (player.GetHealth() <= 0 && computer.GetHealth() <= 0)
                                {
                                    scores[j, k] += 10;
                                    scores[k, j] -= 10;
                                }
                                else if (player.GetHealth() <= 0 && computer.GetHealth() > 0)
                                {
                                    scores[j, k] += 5;
                                    scores[k, j] -= 5;
                                }
                            }
                        }
                    }
                }
                // Affiche les statistiques sous forme de tableau à deux entrées.
                display.DisplayStats(scores);
                // Permet d'attendre avant de fermer le programme pour laisser à l'utilisateur le temps de lire.
                Console.ReadLine();
            }
        }

        /** Méthode permettant de récupérer le choix de l'Intelligence artificielle 
         *  pour des niveaux de difficulté différents.
         *  <returns> le code de l'action choisie par l'IA: 1 Attaque / 2 Défense / 3 Spécial </returns>
         */
        int IAChoices()
        {
            // Choix de l'action de l'IA.
            if (difficulty == 2) return rand.Next(1, 4);
            else
            {
                switch (playerAction)
                {
                    // Le joueur attaque.
                    case 1:
                        if (difficulty == 3)
                        {
                            if (computer.GetName() == "Tank")
                            {
                                if (computer.GetHealth() - 1 > player.GetDamage()) return rand.Next(3) <= 1 ? 3 : rand.Next(1, 4);
                                else if (computer.GetHealth() - 1 > player.GetDamage()) return rand.Next(3) <= 1 ? 1 : rand.Next(1, 4);
                                else return rand.Next(3) <= 1 ? 2 : rand.Next(1, 4);
                            }
                            else
                            {
                                if (computer.GetHealth() > player.GetDamage()) return rand.Next(3) <= 1 ? 1 : rand.Next(1, 4);
                                else return rand.Next(3) <= 1 ? 2 : rand.Next(1, 4);
                            }
                        }
                        else return rand.Next(3) <= 1 ? 1 : rand.Next(1, 4);
                        break;

                    case 2:
                        if (difficulty == 3)
                        {
                            if (computer.GetName() == "Damager" || computer.GetName() == "Wizard") return rand.Next(1, 4);
                            else return rand.Next(3) <= 1 ? 3 : rand.Next(1, 4);
                        }
                        else return rand.Next(3) <= 1 ? 2 : rand.Next(1, 4);
                        break;

                    case 3:
                        if (difficulty == 3)
                        {
                            if (player.GetName() == "Warlock" || player.GetName() == "Healer") return rand.Next(3) <= 1 ? 1 : rand.Next(1, 4);
                            else return rand.Next(3) <= 1 ? 2 : rand.Next(1, 4);
                        }
                        else return rand.Next(3) <= 1 ? 2 : rand.Next(1, 4);
                        break;
                }
                return -1;
            }

        }

        /** Méthode permettant de gérer les effets de la capacité spéciale du Tank. */
        void TankSpecial( bool isPlayer)
        {
            Character user, target;
            if (isPlayer)
            {
                user = player;
                target = computer;
            }
            else
            {
                user = computer;
                target = player;
            }
            user.SpecialCapacity();
            target.Hit(user.GetDamage());
            user.SpecialCapacity();
        }
        /** Méthode permettant de gérer les effets de la capacité spéciale du Warlock. */
        string[] WarlockSpecial( bool isPlayer)
        {
            Character user;
            string[] messages;
            if (isPlayer)
            {
                user = player;
                messages = new string[] { "Votre don de sang n'a pas plu aux dieux noirs, ils ont décidé de vous sacrifier...",
                                          "Votre don de sang n'est pas suffisant...",
                                          "Les dieux noirs acceptent votre sang, vous gagnez en puissance !",
                                          "Les dieux noirs vous bénissent, vous êtes maintenant bien plus puissant !"};
            }
            else
            {
                user = computer;
                messages = new string[] { "Votre adversaire viens de se sacrifier aux dieux noirs...",
                                          "Votre adversaire offre son sang... en vain...",
                                          "Votre adversaire semble devenir plus fort !",
                                          "Les dieux noirs ont accordé leur puissance à votre adversaire !"};
            }

            int tmpHealth = user.GetHealth();
            int tmpDamage = user.GetDamage();

            user.SpecialCapacity();
            if (user.GetHealth() == 0) return new string[] { messages[0] };
            if (user.GetHealth() < tmpHealth) return new string[] { messages[1] };
            if (user.GetDamage() + 2 == tmpDamage) return new string[] { messages[2] };
            if (user.GetDamage() > tmpDamage) return new string[] { messages[3] };
            return new string[]{""};
        }

        static void Main()
        {
            Controller ctrl = new();
            ctrl.Game();
        }
    }
}

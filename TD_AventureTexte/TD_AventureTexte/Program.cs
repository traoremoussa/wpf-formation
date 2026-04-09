using System;

namespace TD_AventureTexte
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Bienvenue dans le jeu de forêt");
                string dest = ReadDestination();

                if (string.IsNullOrEmpty(dest))
                {
                    Console.WriteLine("Aucune destination valide saisie. Fin du jeu.");
                    return;
                }

                switch (dest)
                {
                    case "O":
                        Console.WriteLine("Vous êtes dans le château.");

                        string porte = ReadOption("Porte ou Fenêtre (P/F) : ", new[] { "P", "F" });
                        if (string.IsNullOrEmpty(porte))
                        {
                            Console.WriteLine("Aucune option valide. Vous avez perdu.");
                            break;
                        }

                        if (porte == "P")
                        {
                            Console.WriteLine("Vous vous approchez de la porte et vous tombez. Vous avez perdu.");
                        }
                        else // F
                        {
                            Console.WriteLine("Vous êtes dans le couloir.");

                            string cote = ReadOption("Porte gauche ou droite (G/D) : ", new[] { "G", "D" });
                            if (string.IsNullOrEmpty(cote))
                            {
                                Console.WriteLine("Aucune option valide. Vous avez perdu.");
                                break;
                            }

                            if (cote == "G")
                            {
                                Console.WriteLine("Vous vous approchez de la porte et vous tombez. Vous avez perdu.");
                            }
                            else // D
                            {
                                Console.WriteLine("Vous avez gagné, le prince est sauvé !");
                            }
                        }

                        break;
                    case "E":
                        Console.WriteLine("Vous vous approchez d'une falaise et vous tombez. Vous avez perdu.");
                        break;
                    default:
                        Console.WriteLine("Destination inconnue. Vous avez perdu.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur inattendue est survenue : " + ex.Message);
            }
        }

        private static string ReadDestination(int maxAttempts = 3)
        {
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Console.Write("Saisir la destination (O/E) : ");
                string? input = Console.ReadLine();

                if (input is null)
                {
                    Console.WriteLine("Lecture interrompue (entrée null). Réessayez.");
                    continue;
                }

                string normalized = input.Trim().ToUpperInvariant();

                if (normalized == "O" || normalized == "E")
                    return normalized;

                // Accepter des mots complets comme "OUEST" / "EST"
                if (normalized.StartsWith("O"))
                    return "O";
                if (normalized.StartsWith("E"))
                    return "E";

                Console.WriteLine("Entrée invalide. Tapez 'O' pour Ouest/château ou 'E' pour Est/falaise.");
            }

            return string.Empty;
        }

        private static string ReadOption(string prompt, string[] validOptions, int maxAttempts = 3)
        {
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (input is null)
                {
                    Console.WriteLine("Lecture interrompue (entrée null). Réessayez.");
                    continue;
                }

                string normalized = input.Trim().ToUpperInvariant();

                foreach (var opt in validOptions)
                {
                    if (normalized == opt || normalized.StartsWith(opt))
                        return opt;
                }

                Console.WriteLine("Option invalide. Options attendues : " + string.Join("/", validOptions));
            }

            return string.Empty;
        }
    }
}
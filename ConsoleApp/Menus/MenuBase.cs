using System;
using System.Threading.Tasks;

namespace ConsoleApp.Menus
{
    /// <summary>
    /// Base klasse for alle menuer i applikationen
    /// </summary>
    public abstract class MenuBase
    {
        /// <summary>
        /// Viser menuen og håndterer brugerens valg
        /// </summary>
        public abstract Task ShowAsync();

        /// <summary>
        /// Viser en fejlbesked og venter på brugerinput
        /// </summary>
        protected void ShowError(string message)
        {
            Console.WriteLine($"\nFejl: {message}");
            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        /// <summary>
        /// Viser en succesbesked og venter på brugerinput
        /// </summary>
        protected void ShowSuccess(string message)
        {
            Console.WriteLine($"\n{message}");
            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        /// <summary>
        /// Rydder konsollen og viser en titel
        /// </summary>
        protected void ShowHeader(string title)
        {
            Console.Clear();
            Console.WriteLine($"=== {title} ===");
        }

        /// <summary>
        /// Håndterer en generel fejl
        /// </summary>
        protected void HandleException(Exception ex)
        {
            ShowError(ex.Message);
        }
    }
} 
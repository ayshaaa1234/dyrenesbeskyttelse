using System;
using System.Threading.Tasks;
using ClassLibrary.Services;
using ClassLibrary.Models;

namespace ConsoleApp.Menus
{
    /// <summary>
    /// Menu til håndtering af dyr
    /// </summary>
    public class AnimalMenu : MenuBase
    {
        private readonly AnimalService _animalService;

        public AnimalMenu(AnimalService animalService)
        {
            _animalService = animalService;
        }

        public override async Task ShowAsync()
        {
            while (true)
            {
                ShowHeader("Dyr");
                Console.WriteLine("1. Vis alle dyr");
                Console.WriteLine("2. Tilføj nyt dyr");
                Console.WriteLine("3. Opdater dyr");
                Console.WriteLine("4. Slet dyr");
                Console.WriteLine("5. Søg efter dyr");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.Write("\nVælg en mulighed: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowAllAnimals();
                            break;
                        case "2":
                            await AddNewAnimal();
                            break;
                        case "3":
                            await UpdateAnimal();
                            break;
                        case "4":
                            await DeleteAnimal();
                            break;
                        case "5":
                            await SearchAnimals();
                            break;
                        case "0":
                            return;
                        default:
                            ShowError("Ugyldigt valg");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        private async Task ShowAllAnimals()
        {
            ShowHeader("Alle dyr");
            var animals = await _animalService.GetAllAnimalsAsync();
            
            if (!animals.Any())
            {
                Console.WriteLine("Ingen dyr fundet.");
                Console.WriteLine("\nTryk på en tast for at fortsætte...");
                Console.ReadKey();
                return;
            }

            foreach (var animal in animals)
            {
                DisplayAnimalInfo(animal);
                Console.WriteLine(new string('-', 50));
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task AddNewAnimal()
        {
            ShowHeader("Tilføj nyt dyr");

            Console.Write("Navn: ");
            var name = Console.ReadLine() ?? string.Empty;

            Console.Write("Art: ");
            Console.WriteLine("\nVælg art:");
            Console.WriteLine("1. Hund");
            Console.WriteLine("2. Kat");
            Console.WriteLine("3. Kanin");
            Console.WriteLine("4. Fugl");
            Console.WriteLine("5. Andet");
            Console.Write("Valg: ");
            
            Species species;
            switch (Console.ReadLine())
            {
                case "1": species = Species.Hund; break;
                case "2": species = Species.Kat; break;
                case "3": species = Species.Kanin; break;
                case "4": species = Species.Fugl; break;
                case "5": species = Species.Andet; break;
                default:
                    ShowError("Ugyldigt valg");
                    return;
            }

            Console.Write("Fødselsdato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime birthDate))
            {
                ShowError("Ugyldig dato");
                return;
            }

            Console.Write("Beskrivelse: ");
            var description = Console.ReadLine() ?? string.Empty;

            var animal = new Animal
            {
                Name = name,
                Species = species,
                BirthDate = birthDate,
                Description = description
            };

            await _animalService.CreateAnimalAsync(animal);
            ShowSuccess("Dyr tilføjet succesfuldt!");
        }

        private async Task UpdateAnimal()
        {
            ShowHeader("Opdater dyr");

            Console.Write("Indtast dyrets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                ShowError("Dyr ikke fundet");
                return;
            }

            Console.WriteLine("\nNuværende information:");
            DisplayAnimalInfo(animal);
            Console.WriteLine("\nIndtast ny information (tryk Enter for at beholde nuværende værdi):");

            Console.Write($"Navn [{animal.Name}]: ");
            var name = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(name))
                animal.Name = name;

            Console.Write("Art: ");
            Console.WriteLine("\nVælg art:");
            Console.WriteLine("1. Hund");
            Console.WriteLine("2. Kat");
            Console.WriteLine("3. Kanin");
            Console.WriteLine("4. Fugl");
            Console.WriteLine("5. Andet");
            Console.Write("Valg: ");
            
            var speciesChoice = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(speciesChoice))
            {
                switch (speciesChoice)
                {
                    case "1": animal.Species = Species.Hund; break;
                    case "2": animal.Species = Species.Kat; break;
                    case "3": animal.Species = Species.Kanin; break;
                    case "4": animal.Species = Species.Fugl; break;
                    case "5": animal.Species = Species.Andet; break;
                    default:
                        ShowError("Ugyldigt valg");
                        return;
                }
            }

            Console.Write($"Fødselsdato [{animal.BirthDate:dd/MM/yyyy}]: ");
            var birthDateStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(birthDateStr) && DateTime.TryParse(birthDateStr, out DateTime birthDate))
                animal.BirthDate = birthDate;

            Console.Write($"Beskrivelse [{animal.Description}]: ");
            var description = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(description))
                animal.Description = description;

            await _animalService.UpdateAnimalAsync(animal);
            ShowSuccess("Dyr opdateret succesfuldt!");
        }

        private async Task DeleteAnimal()
        {
            ShowHeader("Slet dyr");

            Console.Write("Indtast dyrets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var animal = await _animalService.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                ShowError("Dyr ikke fundet");
                return;
            }

            Console.WriteLine("\nEr du sikker på, at du vil slette dette dyr?");
            DisplayAnimalInfo(animal);
            Console.Write("\nSkriv 'JA' for at bekræfte: ");
            
            if (Console.ReadLine()?.ToUpper() != "JA")
            {
                Console.WriteLine("Sletning annulleret.");
                Console.WriteLine("\nTryk på en tast for at fortsætte...");
                Console.ReadKey();
                return;
            }

            await _animalService.DeleteAnimalAsync(id);
            ShowSuccess("Dyr slettet succesfuldt!");
        }

        private async Task SearchAnimals()
        {
            while (true)
            {
                ShowHeader("Søg efter dyr");
                Console.WriteLine("1. Søg efter navn");
                Console.WriteLine("2. Søg efter art");
                Console.WriteLine("3. Søg efter alder");
                Console.WriteLine("0. Tilbage");

                Console.Write("\nVælg en mulighed: ");
                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await SearchByName();
                            break;
                        case "2":
                            await SearchBySpecies();
                            break;
                        case "3":
                            await SearchByAge();
                            break;
                        case "0":
                            return;
                        default:
                            ShowError("Ugyldigt valg");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        private async Task SearchByName()
        {
            ShowHeader("Søg efter navn");
            Console.Write("Indtast navn: ");
            var name = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowError("Navn kan ikke være tomt");
                return;
            }

            var animals = await _animalService.GetAnimalsByNameAsync(name);
            DisplaySearchResults(animals);
        }

        private async Task SearchBySpecies()
        {
            ShowHeader("Søg efter art");
            Console.WriteLine("Vælg art:");
            Console.WriteLine("1. Hund");
            Console.WriteLine("2. Kat");
            Console.WriteLine("3. Kanin");
            Console.WriteLine("4. Fugl");
            Console.WriteLine("5. Andet");
            Console.Write("Valg: ");

            Species species;
            switch (Console.ReadLine())
            {
                case "1": species = Species.Hund; break;
                case "2": species = Species.Kat; break;
                case "3": species = Species.Kanin; break;
                case "4": species = Species.Fugl; break;
                case "5": species = Species.Andet; break;
                default:
                    ShowError("Ugyldigt valg");
                    return;
            }

            var animals = await _animalService.GetAnimalsBySpeciesAsync(species);
            DisplaySearchResults(animals);
        }

        private async Task SearchByAge()
        {
            ShowHeader("Søg efter alder");
            Console.Write("Indtast minimumsalder (år): ");
            if (!int.TryParse(Console.ReadLine(), out int minAge))
            {
                ShowError("Ugyldig alder");
                return;
            }

            Console.Write("Indtast maksimumsalder (år): ");
            if (!int.TryParse(Console.ReadLine(), out int maxAge))
            {
                ShowError("Ugyldig alder");
                return;
            }

            var animals = await _animalService.GetAnimalsByAgeRangeInYearsAsync(minAge, maxAge);
            DisplaySearchResults(animals);
        }

        private void DisplaySearchResults(IEnumerable<Animal> animals)
        {
            if (!animals.Any())
            {
                Console.WriteLine("Ingen dyr fundet.");
            }
            else
            {
                foreach (var animal in animals)
                {
                    DisplayAnimalInfo(animal);
                    Console.WriteLine(new string('-', 50));
                }
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private void DisplayAnimalInfo(Animal animal)
        {
            Console.WriteLine($"ID: {animal.Id}");
            Console.WriteLine($"Navn: {animal.Name}");
            Console.WriteLine($"Art: {animal.Species}");
            Console.WriteLine($"Fødselsdato: {animal.BirthDate:dd/MM/yyyy}");
            Console.WriteLine($"Alder: {animal.GetFormattedAge()}");
            Console.WriteLine($"Beskrivelse: {animal.Description}");
        }
    }
} 
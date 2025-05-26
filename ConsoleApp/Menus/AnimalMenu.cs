using System;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Models; // For Animal, HealthRecord, Visit
using ClassLibrary.Features.AnimalManagement.Core.Enums; // For AnimalStatus, Species, VisitStatus

namespace ConsoleApp.Menus
{
    public class AnimalMenu
    {
        private readonly IAnimalManagementService _animalManagementService;

        public AnimalMenu(IAnimalManagementService animalManagementService)
        {
            _animalManagementService = animalManagementService;
        }

        public async Task ShowAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Dyreadministration");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("1. Vis alle dyr");
                Console.WriteLine("2. Tilføj nyt dyr");
                Console.WriteLine("3. Opdater dyr");
                Console.WriteLine("4. Slet dyr");
                Console.WriteLine("5. Se sundhedsjournal for et dyr");
                Console.WriteLine("6. Tilføj sundhedsnotat");
                Console.WriteLine("7. Registrer besøg");
                Console.WriteLine("8. Vis dyr der skal vaccineres");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.WriteLine("----------------------------------------");
                Console.Write("Tag et valg: ");

                string? choice = Console.ReadLine();
                switch (choice)

                {
                    case "1":
                        await ListAllAnimalsAsync();
                        break;
                    case "2":
                        await AddAnimalAsync();
                        break;
                    case "3":
                        await UpdateAnimalAsync();
                        break;
                    case "4":
                        await DeleteAnimalAsync();
                        break;
                    case "5":
                        await ViewHealthRecordsAsync();
                        break;
                    case "6":
                        await AddHealthRecordAsync();
                        break;
                    case "7":
                        await AddVisitAsync();
                        break;
                    case "8":
                        await ListAnimalsNeedingVaccinationAsync();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Ugyldigt valg. Prøv igen.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private async Task ListAllAnimalsAsync()
        {
            Console.Clear();
            Console.WriteLine("Alle registrerede dyr:");
            var animals = await _animalManagementService.GetAllAnimalsAsync();
            if (!animals.Any())
            {
                Console.WriteLine("Ingen dyr fundet.");
            }
            else
            {
                foreach (var animal in animals)
                {
                    Console.WriteLine($"- ID: {animal.Id}, Navn: {animal.Name}, Art: {animal.Species}, Status: {animal.Status}");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task AddAnimalAsync()
        {
            Console.Clear();
            Console.WriteLine("Tilføj nyt dyr");

            Console.Write("Navn: ");
            string? name = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Navn må ikke være tomt. Prøv igen.");
                Console.Write("Navn: ");
                name = Console.ReadLine();
            }

            Console.Write("Beskrivelse: ");
            string? description = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Beskrivelse må ikke være tom. Prøv igen.");
                Console.Write("Beskrivelse: ");
                description = Console.ReadLine();
            }

            Species species = GetSpeciesFromUserInput();
            AnimalStatus status = GetAnimalStatusFromUserInput();
            DateTime birthDate = GetDateTimeFromUserInput("Fødselsdato (YYYY-MM-DD): ");
            
            Console.Write("Race: ");
            string? breed = Console.ReadLine() ?? string.Empty;

            decimal weight = GetDecimalFromUserInput("Vægt (kg): ");

            Console.Write("Sundhedsstatus (tekst): ");
            string? healthStatusText = Console.ReadLine() ?? string.Empty;

            Console.Write("Billed-URL (valgfri): ");
            string? pictureUrl = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(pictureUrl))
            {
                pictureUrl = null;
            }

            var newAnimal = new Animal
            {
                Name = name,
                Description = description,
                BirthDate = birthDate,
                Species = species,
                IntakeDate = DateTime.UtcNow,
                Status = status,
                PictureUrl = pictureUrl,
                Breed = breed,
                Gender = GetGenderFromUserInput(),
                Weight = weight,
                HealthStatus = healthStatusText,
                IsAdopted = false,
                IsDeleted = false
            };

            try
            {
                var createdAnimal = await _animalManagementService.CreateAnimalAsync(newAnimal);
                Console.WriteLine($"\nDyret '{createdAnimal.Name}' blev tilføjet med ID: {createdAnimal.Id}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFejl ved tilføjelse af dyr: {ex.Message}");
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private Species GetSpeciesFromUserInput()
        {
            Console.WriteLine("Vælg art:");
            foreach (int i in Enum.GetValues(typeof(Species)))
            {
                Console.WriteLine($"{i}. {Enum.GetName(typeof(Species), i)}");
            }
            Console.Write("Art: ");
            string? input = Console.ReadLine();
            Species species;
            while (!Enum.TryParse(input, out species) || !Enum.IsDefined(typeof(Species), species))
            {
                Console.WriteLine("Ugyldig art. Prøv igen.");
                Console.Write("Art: ");
                input = Console.ReadLine();
            }
            return species;
        }

        private AnimalStatus GetAnimalStatusFromUserInput()
        {
            Console.WriteLine("Vælg status:");
            foreach (int i in Enum.GetValues(typeof(AnimalStatus)))
            {
                Console.WriteLine($"{i}. {Enum.GetName(typeof(AnimalStatus), i)}");
            }
            Console.Write("Status: ");
            string? input = Console.ReadLine();
            AnimalStatus status;
            while (!Enum.TryParse(input, out status) || !Enum.IsDefined(typeof(AnimalStatus), status))
            {
                Console.WriteLine("Ugyldig status. Prøv igen.");
                Console.Write("Status: ");
                input = Console.ReadLine();
            }
            return status;
        }
        
        private VisitStatus GetVisitStatusFromUserInput()
        {
            Console.WriteLine("Vælg besøgsstatus:");
            foreach (int i in Enum.GetValues(typeof(VisitStatus)))
            {
                Console.WriteLine($"{i}. {Enum.GetName(typeof(VisitStatus), i)}");
            }
            Console.Write("Status: ");
            string? input = Console.ReadLine();
            VisitStatus status;
            while (!Enum.TryParse(input, out status) || !Enum.IsDefined(typeof(VisitStatus), status))
            {
                Console.WriteLine("Ugyldig status. Prøv igen.");
                Console.Write("Status: ");
                input = Console.ReadLine();
            }
            return status;
        }

        private DateTime GetDateTimeFromUserInput(string prompt)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            DateTime date;
            while (!DateTime.TryParse(input, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date) && 
                   !DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date) &&
                   !DateTime.TryParseExact(input, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date) 
                  )
            {
                Console.WriteLine("Ugyldigt datoformat. Brug venligst YYYY-MM-DD eller YYYY-MM-DD HH:mm. Prøv igen.");
                Console.Write(prompt);
                input = Console.ReadLine();
            }
            return date;
        }

        private decimal GetDecimalFromUserInput(string prompt)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            decimal value;
            while (!decimal.TryParse(input, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value))
            {
                Console.WriteLine("Ugyldigt decimaltal. Prøv igen.");
                Console.Write(prompt);
                input = Console.ReadLine();
            }
            return value;
        }
        
        private int GetIntFromUserInput(string prompt)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            int value;
            while (!int.TryParse(input, out value))
            {
                Console.WriteLine("Ugyldigt heltal. Prøv igen.");
                Console.Write(prompt);
                input = Console.ReadLine();
            }
            return value;
        }

        private async Task UpdateAnimalAsync()
        {
            Console.Clear();
            Console.WriteLine("Opdater dyr");
            int animalId = GetIntFromUserInput("Indtast ID (tal) på dyret der skal opdateres: ");

            var animalToUpdate = await _animalManagementService.GetAnimalByIdAsync(animalId);
            if (animalToUpdate == null)
            {
                Console.WriteLine($"Dyr med ID {animalId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nOpdaterer dyr: {animalToUpdate.Name} (ID: {animalToUpdate.Id})");
            Console.WriteLine($"Nuværende værdier vises i parentes. Tryk Enter for at beholde nuværende værdi.");

            Console.Write($"Navn ({animalToUpdate.Name}): ");
            string? name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) animalToUpdate.Name = name;

            Console.Write($"Beskrivelse ({animalToUpdate.Description}): ");
            string? description = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(description)) animalToUpdate.Description = description;
            
            Console.WriteLine($"Nuværende art: {animalToUpdate.Species}. Ønsker du at ændre? (j/n)");
            if (Console.ReadLine()?.ToLower() == "j")
            {
                animalToUpdate.Species = GetSpeciesFromUserInput();
            }

            Console.WriteLine($"Nuværende status: {animalToUpdate.Status}. Ønsker du at ændre? (j/n)");
            if (Console.ReadLine()?.ToLower() == "j")
            {
                animalToUpdate.Status = GetAnimalStatusFromUserInput();
            }
            
            Console.Write($"Fødselsdato ({animalToUpdate.BirthDate:yyyy-MM-dd}): ");
            string? birthDateInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(birthDateInput))
            {
                if (DateTime.TryParseExact(birthDateInput, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime birthDate))
                {
                     animalToUpdate.BirthDate = birthDate;
                }
                else
                {
                    Console.WriteLine("Ugyldigt datoformat. Fødselsdato ikke ændret.");
                }
            }
            
            Console.Write($"Race ({animalToUpdate.Breed}): ");
            string? breed = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(breed)) animalToUpdate.Breed = breed;

            Console.WriteLine($"Nuværende køn: {animalToUpdate.Gender}. Ønsker du at ændre? (j/n)");
            if (Console.ReadLine()?.ToLower() == "j")
            {
                animalToUpdate.Gender = GetGenderFromUserInput();
            }

            Console.Write($"Vægt ({animalToUpdate.Weight} kg): ");
            string? weightInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(weightInput) && decimal.TryParse(weightInput, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal weightValue))
            {
                animalToUpdate.Weight = weightValue;
            }
            else if(!string.IsNullOrWhiteSpace(weightInput))
            {
                 Console.WriteLine("Ugyldigt vægtformat. Vægt ikke ændret.");
            }

            Console.Write($"Sundhedsstatus ({animalToUpdate.HealthStatus}): ");
            string? healthStatusText = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(healthStatusText)) animalToUpdate.HealthStatus = healthStatusText;

            Console.Write($"Billed-URL ({animalToUpdate.PictureUrl ?? "Ingen"}): ");
            string? pictureUrl = Console.ReadLine();
            if (pictureUrl != null)
            {
                animalToUpdate.PictureUrl = string.IsNullOrWhiteSpace(pictureUrl) ? null : pictureUrl;
            }

            try
            {
                await _animalManagementService.UpdateAnimalAsync(animalToUpdate);
                Console.WriteLine($"\nDyret '{animalToUpdate.Name}' blev opdateret.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFejl ved opdatering af dyr: {ex.Message}");
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task DeleteAnimalAsync()
        {
            Console.Clear();
            Console.WriteLine("Slet dyr");
            int animalId = GetIntFromUserInput("Indtast ID (tal) på dyret der skal slettes: ");
            
            var animalToDelete = await _animalManagementService.GetAnimalByIdAsync(animalId);
            if (animalToDelete == null)
            {
                Console.WriteLine($"Dyr med ID {animalId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Er du sikker på, at du vil slette {animalToDelete.Name} (ID: {animalToDelete.Id})? (j/n)");
            string? confirmation = Console.ReadLine();
            if (confirmation?.ToLower() == "j")
            {
                try
                {
                    await _animalManagementService.DeleteAnimalAsync(animalId);
                    Console.WriteLine($"Dyret '{animalToDelete.Name}' blev slettet.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fejl ved sletning af dyr: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Sletning annulleret.");
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task ViewHealthRecordsAsync()
        {
            Console.Clear();
            Console.WriteLine("Se sundhedsjournal");
            int animalId = GetIntFromUserInput("Indtast ID (tal) på dyret: ");

            var animal = await _animalManagementService.GetAnimalByIdAsync(animalId);
            if (animal == null)
            {
                Console.WriteLine($"Dyr med ID {animalId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nSundhedsjournal for {animal.Name} (ID: {animal.Id}):");
            var healthRecords = await _animalManagementService.GetHealthRecordsByAnimalIdAsync(animalId);
            if (!healthRecords.Any())
            {
                Console.WriteLine("Ingen sundhedsnotater fundet for dette dyr.");
            }
            else
            {
                foreach (var record in healthRecords)
                {
                    Console.WriteLine($"- ID: {record.Id}, Dato: {record.RecordDate:yyyy-MM-dd}, Dyrlæge: {record.VeterinarianName}");
                    Console.WriteLine($"  Diagnose: {record.Diagnosis}");
                    Console.WriteLine($"  Behandling: {record.Treatment}");
                    Console.WriteLine($"  Medicin: {record.Medication}");
                    Console.WriteLine($"  Vægt: {record.Weight} kg");
                    Console.WriteLine($"  Noter: {record.Notes}");
                    Console.WriteLine($"  Vaccineret: {(record.IsVaccinated ? "Ja" : "Nej")}, Næste vaccine: {record.NextVaccinationDate?.ToString("yyyy-MM-dd") ?? "N/A"}");
                }
            }
            Console.WriteLine("Tryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task AddHealthRecordAsync()
        {
            Console.Clear();
            Console.WriteLine("Tilføj sundhedsnotat");
            int animalId = GetIntFromUserInput("Indtast ID (tal) på dyret: ");
            
            var animal = await _animalManagementService.GetAnimalByIdAsync(animalId);
            if (animal == null)
            {
                Console.WriteLine($"Dyr med ID {animalId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }
            
            Console.WriteLine($"Tilføjer sundhedsnotat for {animal.Name}");

            DateTime recordDate = GetDateTimeFromUserInput("Dato for notat (YYYY-MM-DD): ");
            
            Console.Write("Dyrlæge Navn: ");
            string? vetName = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(vetName))
            {
                Console.WriteLine("Dyrlægenavn må ikke være tomt. Prøv igen.");
                Console.Write("Dyrlæge Navn: ");
                vetName = Console.ReadLine();
            }

            Console.Write("Diagnose: ");
            string? diagnosis = Console.ReadLine() ?? string.Empty;
            
            Console.Write("Udført behandling: ");
            string? treatment = Console.ReadLine() ?? string.Empty;

            Console.Write("Medicin: ");
            string? medication = Console.ReadLine() ?? string.Empty;

            decimal weight = GetDecimalFromUserInput("Dyrets vægt (kg) ved dette notat: ");
            
            Console.Write("Noter (valgfri): ");
            string? notes = Console.ReadLine() ?? string.Empty;

            Console.Write("Er dyret vaccineret ved dette notat? (j/n): ");
            bool isVaccinated = Console.ReadLine()?.ToLower() == "j";
            DateTime? nextVaccinationDate = null;
            if (isVaccinated)
            {
                 Console.Write("Næste vaccinationsdato (YYYY-MM-DD, valgfri, tryk enter for ingen): ");
                 string? nextDateInput = Console.ReadLine();
                 if (!string.IsNullOrWhiteSpace(nextDateInput) && DateTime.TryParseExact(nextDateInput, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime nextDate))
                 {
                     nextVaccinationDate = nextDate;
                 }
            }

            var newRecord = new HealthRecord
            {
                AnimalId = animalId,
                RecordDate = recordDate,
                VeterinarianName = vetName,
                Diagnosis = diagnosis,
                Treatment = treatment,
                Medication = medication,
                Weight = weight,
                Notes = notes,
                IsVaccinated = isVaccinated,
                NextVaccinationDate = nextVaccinationDate,
                CreatedAt = DateTime.UtcNow 
            };

            try
            {
                var createdRecord = await _animalManagementService.AddHealthRecordAsync(animalId, newRecord);
                Console.WriteLine($"\nSundhedsnotat tilføjet med ID: {createdRecord.Id}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFejl ved tilføjelse af sundhedsnotat: {ex.Message}");
            }
            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task AddVisitAsync()
        {
            Console.Clear();
            Console.WriteLine("Registrer besøg");
            int animalId = GetIntFromUserInput("Indtast ID (tal) på dyret: ");
            
            var animal = await _animalManagementService.GetAnimalByIdAsync(animalId);
            if (animal == null)
            {
                Console.WriteLine($"Dyr med ID {animalId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"Registrerer besøg for {animal.Name}");

            DateTime plannedDate = GetDateTimeFromUserInput("Planlagt dato for besøg (YYYY-MM-DD HH:mm): "); 
             
            Console.Write("Planlagt varighed (minutter): ");
            int plannedDuration = GetIntFromUserInput("Planlagt varighed (minutter): ");

            Console.Write("Type af besøg (f.eks. Kennelscreening, Adoptionssamtale): ");
            string? visitType = Console.ReadLine() ?? string.Empty;
            
            Console.Write("Formål med besøget: ");
            string? purpose = Console.ReadLine() ?? string.Empty;
            
            Console.Write("Navn på besøgende: ");
            string? visitorName = Console.ReadLine() ?? string.Empty;

            VisitStatus visitStatus = GetVisitStatusFromUserInput();
            
            Console.Write("Noter (valgfri): ");
            string? notes = Console.ReadLine() ?? string.Empty;

            var newVisit = new Visit
            {
                AnimalId = animalId,
                PlannedDate = plannedDate,
                PlannedDuration = plannedDuration,
                Type = visitType,
                Purpose = purpose,
                Visitor = visitorName,
                Status = visitStatus, 
                Notes = notes
            };

            try
            {
                var createdVisit = await _animalManagementService.CreateVisitAsync(newVisit);
                Console.WriteLine($"\nBesøg registreret med ID: {createdVisit.Id}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFejl ved registrering af besøg: {ex.Message}");
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task ListAnimalsNeedingVaccinationAsync()
        {
            Console.Clear();
            Console.WriteLine("Dyr der skal vaccineres:");
            
            try
            {
                var animals = await _animalManagementService.GetAnimalsNeedingVaccinationAsync();
                if (!animals.Any())
                {
                    Console.WriteLine("Ingen dyr fundet, der har brug for vaccination ifølge de nuværende kriterier.");
                }
                else
                {
                    Console.WriteLine("Følgende dyr har brug for vaccination (baseret på service logik):");
                    foreach (var animal in animals)
                    {
                        Console.WriteLine($"- ID: {animal.Id}, Navn: {animal.Name}, Art: {animal.Species}");
                    }
                }
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("Funktionaliteten 'Vis dyr der skal vaccineres' er endnu ikke fuldt implementeret i service-laget.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved hentning af dyr til vaccination: {ex.Message}");
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private Gender GetGenderFromUserInput()
        {
            Console.WriteLine("Vælg køn:");
            foreach (int i in Enum.GetValues(typeof(Gender)))
            {
                Console.WriteLine($"{i}. {Enum.GetName(typeof(Gender), i)}");
            }
            Console.Write("Køn: ");
            string? input = Console.ReadLine();
            Gender gender;
            while (string.IsNullOrWhiteSpace(input) || !Enum.TryParse(input.Trim(), true, out gender) || !Enum.IsDefined(typeof(Gender), gender))
            {
                Console.WriteLine("Ugyldigt køn. Prøv igen.");
                Console.Write("Køn: ");
                input = Console.ReadLine();
            }
            return gender;
        }
    }
} 
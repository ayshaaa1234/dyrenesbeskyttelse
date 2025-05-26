using System;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.Adoptions.Application.Abstractions;
using ClassLibrary.Features.Adoptions.Core.Models;
using ClassLibrary.Features.Adoptions.Core.Enums;
using ClassLibrary.Features.AnimalManagement.Application.Abstractions;
using ClassLibrary.Features.Customers.Application.Abstractions;
using ClassLibrary.Features.AnimalManagement.Core.Enums;

namespace ConsoleApp.Menus
{
    public class AdoptionMenu
    {
        private readonly IAdoptionService _adoptionService;
        private readonly IAnimalManagementService _animalManagementService;
        private readonly ICustomerService _customerService;

        public AdoptionMenu(IAdoptionService adoptionService, IAnimalManagementService animalManagementService, ICustomerService customerService)
        {
            _adoptionService = adoptionService;
            _animalManagementService = animalManagementService;
            _customerService = customerService;
        }

        public async Task ShowAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Adoptionsadministration");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("1. Vis alle adoptioner");
                Console.WriteLine("2. Opret ny adoptionsansøgning");
                Console.WriteLine("3. Godkend adoptionsansøgning");
                Console.WriteLine("4. Afvis adoptionsansøgning");
                Console.WriteLine("5. Gennemfør adoption");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.WriteLine("----------------------------------------");
                Console.Write("Tag et valg: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ListAllAdoptionsAsync();
                        break;
                    case "2":
                        await CreateAdoptionAsync();
                        break;
                    case "3":
                        await ApproveAdoptionAsync();
                        break;
                    case "4":
                        await RejectAdoptionAsync();
                        break;
                    case "5":
                        await CompleteAdoptionAsync();
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

        private async Task ListAllAdoptionsAsync()
        {
            Console.Clear();
            Console.WriteLine("Alle registrerede adoptioner:");
            var adoptions = await _adoptionService.GetAllAdoptionsAsync();
            if (!adoptions.Any())
            {
                Console.WriteLine("Ingen adoptioner fundet.");
            }
            else
            {
                foreach (var adoption in adoptions)
                {
                    string animalName = "Ukendt dyr";
                    var animal = await _animalManagementService.GetAnimalByIdAsync(adoption.AnimalId);
                    if (animal != null) animalName = animal.Name;

                    string customerName = "Ukendt kunde";
                    var customer = await _customerService.GetByIdAsync(adoption.CustomerId);
                    if (customer != null) customerName = $"{customer.FirstName} {customer.LastName}";
                    
                    Console.WriteLine($"- ID: {adoption.Id}, DyreID: {adoption.AnimalId} ({animalName}), KundeID: {adoption.CustomerId} ({customerName}), Status: {adoption.Status}, Ansøgningsdato: {adoption.ApplicationDate:yyyy-MM-dd}");
                }
            }
            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task CreateAdoptionAsync()
        {
            Console.Clear();
            Console.WriteLine("Opret ny adoptionsansøgning");

            int animalId = GetIntFromUserInput("Indtast ID (tal) på dyret der ønskes adopteret: ");
            var animalToAdopt = await _animalManagementService.GetAnimalByIdAsync(animalId);
            if (animalToAdopt == null)
            {
                Console.WriteLine($"Dyr med ID {animalId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }
            if (animalToAdopt.Status != AnimalStatus.Available) 
            {
                Console.WriteLine($"Dyr '{animalToAdopt.Name}' (ID: {animalId}) har status '{animalToAdopt.Status}' og er ikke tilgængeligt for adoption.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"Valgt dyr: {animalToAdopt.Name} (Art: {animalToAdopt.Species})");

            int customerId = GetIntFromUserInput("Indtast ID (tal) på kunden der ansøger: ");
            var applyingCustomer = await _customerService.GetByIdAsync(customerId);
            if (applyingCustomer == null)
            {
                Console.WriteLine($"Kunde med ID {customerId} blev ikke fundet.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"Valgt kunde: {applyingCustomer.FirstName} {applyingCustomer.LastName} (Email: {applyingCustomer.Email})");

            Console.Write("Noter til ansøgningen (valgfri): ");
            string? notes = Console.ReadLine();

            var newAdoption = new Adoption
            {
                AnimalId = animalId,
                CustomerId = customerId,
                ApplicationDate = DateTime.UtcNow,
                Status = AdoptionStatus.Pending, 
                Notes = notes ?? string.Empty
            };

            try
            {
                var createdAdoption = await _adoptionService.CreateAdoptionAsync(newAdoption);
                Console.WriteLine($"\nAdoptionsansøgning oprettet med ID: {createdAdoption.Id} for {animalToAdopt.Name} af {applyingCustomer.FirstName} {applyingCustomer.LastName}.");

                animalToAdopt.Status = AnimalStatus.Reserved; 
                await _animalManagementService.UpdateAnimalAsync(animalToAdopt);
                Console.WriteLine($"Status for {animalToAdopt.Name} er opdateret til {animalToAdopt.Status}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFejl ved oprettelse af adoptionsansøgning: {ex.Message}");
            }
            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task ApproveAdoptionAsync()
        {
            Console.Clear();
            Console.WriteLine("Godkend adoptionsansøgning");
            int adoptionId = GetIntFromUserInput("Indtast ID (tal) på adoptionsansøgningen der skal godkendes: ");

            var adoption = await _adoptionService.GetAdoptionByIdAsync(adoptionId);
            if (adoption == null)
            {
                Console.WriteLine($"Adoption med ID {adoptionId} ikke fundet.");
                Console.ReadKey();
                return;
            }
            if (adoption.Status != AdoptionStatus.Pending)
            {
                Console.WriteLine($"Adoptionen har status {adoption.Status} og kan ikke godkendes (skal være Pending).");
                Console.ReadKey();
                return;
            }

            int employeeId = GetIntFromUserInput("Indtast ID (tal) på medarbejderen der godkender: ");

            try
            {
                await _adoptionService.ApproveAdoptionAsync(adoptionId, employeeId);
                Console.WriteLine($"\nAdoptionsansøgning {adoptionId} blev godkendt af medarbejder {employeeId}.");
                
                var animal = await _animalManagementService.GetAnimalByIdAsync(adoption.AnimalId);
                if(animal != null)
                {
                    animal.Status = AnimalStatus.Reserved;
                    await _animalManagementService.UpdateAnimalAsync(animal);
                    Console.WriteLine($"Status for dyr ID {animal.Id} ({animal.Name}) er opdateret til {animal.Status}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFejl ved godkendelse af adoption: {ex.Message}");
            }
            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task RejectAdoptionAsync()
        {
            Console.Clear();
            Console.WriteLine("Afvis adoptionsansøgning");
            int adoptionId = GetIntFromUserInput("Indtast ID (tal) på adoptionsansøgningen der skal afvises: ");

            var adoption = await _adoptionService.GetAdoptionByIdAsync(adoptionId);
            if (adoption == null)
            {
                Console.WriteLine($"Adoption med ID {adoptionId} ikke fundet.");
                Console.ReadKey();
                return;
            }
            if (adoption.Status != AdoptionStatus.Pending)
            {
                Console.WriteLine($"Adoptionen har status {adoption.Status} og kan ikke afvises (skal være Pending).");
                Console.ReadKey();
                return;
            }

            int employeeId = GetIntFromUserInput("Indtast ID (tal) på medarbejderen der afviser: ");
            Console.Write("Begrundelse for afvisning (valgfri, gemmes i Noter): ");
            string? rejectionReason = Console.ReadLine();

            try
            {
                if (!string.IsNullOrWhiteSpace(rejectionReason))
                {
                    adoption.Notes = string.IsNullOrWhiteSpace(adoption.Notes) 
                        ? $"Afvist af medarbejder {employeeId}: {rejectionReason}" 
                        : $"{adoption.Notes}\nAfvist af medarbejder {employeeId}: {rejectionReason}";
                    await _adoptionService.UpdateAdoptionAsync(adoption);
                }
                await _adoptionService.RejectAdoptionAsync(adoptionId, employeeId); 
                Console.WriteLine($"\nAdoptionsansøgning {adoptionId} blev afvist af medarbejder {employeeId}.");

                var animal = await _animalManagementService.GetAnimalByIdAsync(adoption.AnimalId);
                if(animal != null && animal.Status == AnimalStatus.Reserved)
                {
                    animal.Status = AnimalStatus.Available;
                    await _animalManagementService.UpdateAnimalAsync(animal);
                    Console.WriteLine($"Status for dyr ID {animal.Id} ({animal.Name}) er opdateret til {animal.Status}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFejl ved afvisning af adoption: {ex.Message}");
            }
            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private async Task CompleteAdoptionAsync()
        {
            Console.Clear();
            Console.WriteLine("Gennemfør adoption");
            int adoptionId = GetIntFromUserInput("Indtast ID (tal) på adoptionsansøgningen der skal gennemføres: ");
            
            var adoption = await _adoptionService.GetAdoptionByIdAsync(adoptionId);
            if (adoption == null)
            {
                Console.WriteLine($"Adoption med ID {adoptionId} ikke fundet.");
                Console.ReadKey();
                return;
            }
            if (adoption.Status != AdoptionStatus.Approved)
            {
                Console.WriteLine($"Adoptionen har status {adoption.Status} og kan ikke gennemføres (skal være Approved).");
                Console.ReadKey();
                return;
            }

            DateTime actualAdoptionDate = GetDateTimeFromUserInput("Indtast faktisk dato for adoption (YYYY-MM-DD): ");

            try
            {
                await _adoptionService.CompleteAdoptionAsync(adoptionId); 
                Console.WriteLine($"\nAdoption {adoptionId} blev gennemført.");

                var animal = await _animalManagementService.GetAnimalByIdAsync(adoption.AnimalId);
                if(animal != null)
                {
                    animal.Status = AnimalStatus.Adopted;
                    animal.AdoptionDate = actualAdoptionDate;
                    await _animalManagementService.UpdateAnimalAsync(animal);
                    Console.WriteLine($"Status for dyr ID {animal.Id} ({animal.Name}) er opdateret til {animal.Status}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFejl ved gennemførsel af adoption: {ex.Message}");
            }
            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
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
    }
} 
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Services;
using ClassLibrary.Models;

namespace ConsoleApp.Menus
{
    /// <summary>
    /// Menu til håndtering af adoptioner
    /// </summary>
    public class AdoptionMenu : MenuBase
    {
        private readonly AdoptionService _adoptionService;

        public AdoptionMenu(AdoptionService adoptionService)
        {
            _adoptionService = adoptionService;
        }

        public override async Task ShowAsync()
        {
            while (true)
            {
                ShowHeader("Adoptioner");
                Console.WriteLine("1. Vis alle adoptioner");
                Console.WriteLine("2. Vis adoptioner efter status");
                Console.WriteLine("3. Vis adoptioner efter dato");
                Console.WriteLine("4. Vis adoptioner efter dyr");
                Console.WriteLine("5. Vis adoptioner efter kunde");
                Console.WriteLine("6. Opret ny adoption");
                Console.WriteLine("7. Opdater adoption");
                Console.WriteLine("8. Slet adoption");
                Console.WriteLine("9. Godkend adoption");
                Console.WriteLine("10. Afvis adoption");
                Console.WriteLine("11. Gennemfør adoption");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.Write("\nVælg en mulighed: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowAllAdoptions();
                            break;
                        case "2":
                            await ShowAdoptionsByStatus();
                            break;
                        case "3":
                            await ShowAdoptionsByDate();
                            break;
                        case "4":
                            await ShowAdoptionsByAnimal();
                            break;
                        case "5":
                            await ShowAdoptionsByCustomer();
                            break;
                        case "6":
                            await CreateNewAdoption();
                            break;
                        case "7":
                            await UpdateAdoption();
                            break;
                        case "8":
                            await DeleteAdoption();
                            break;
                        case "9":
                            await ApproveAdoption();
                            break;
                        case "10":
                            await RejectAdoption();
                            break;
                        case "11":
                            await CompleteAdoption();
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

        private async Task ShowAllAdoptions()
        {
            ShowHeader("Alle adoptioner");
            var adoptions = await _adoptionService.GetAllAdoptionsAsync();
            DisplayAdoptions(adoptions);
        }

        private async Task ShowAdoptionsByStatus()
        {
            ShowHeader("Adoptioner efter status");
            Console.WriteLine("Vælg status:");
            Console.WriteLine("1. Under behandling");
            Console.WriteLine("2. Godkendt");
            Console.WriteLine("3. Afvist");
            Console.WriteLine("4. Gennemført");
            Console.Write("Valg: ");

            AdoptionStatus status;
            switch (Console.ReadLine())
            {
                case "1": status = AdoptionStatus.Pending; break;
                case "2": status = AdoptionStatus.Approved; break;
                case "3": status = AdoptionStatus.Rejected; break;
                case "4": status = AdoptionStatus.Completed; break;
                default:
                    ShowError("Ugyldigt valg");
                    return;
            }

            try
            {
                var adoptions = await _adoptionService.GetAdoptionsByStatusAsync(status);
                DisplayAdoptions(adoptions);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async Task ShowAdoptionsByDate()
        {
            ShowHeader("Adoptioner efter dato");
            
            DateTime startDate;
            DateTime endDate;

            while (true)
            {
                Console.Write("Indtast startdato (dd/mm/yyyy): ");
                if (!DateTime.TryParse(Console.ReadLine(), out startDate))
                {
                    ShowError("Ugyldig startdato. Prøv igen.");
                    continue;
                }
                break;
            }

            while (true)
            {
                Console.Write("Indtast slutdato (dd/mm/yyyy): ");
                if (!DateTime.TryParse(Console.ReadLine(), out endDate))
                {
                    ShowError("Ugyldig slutdato. Prøv igen.");
                    continue;
                }
                if (endDate < startDate)
                {
                    ShowError("Slutdato skal være efter startdato. Prøv igen.");
                    continue;
                }
                break;
            }

            try
            {
                var adoptions = await _adoptionService.GetAdoptionsByDateRangeAsync(startDate, endDate);
                DisplayAdoptions(adoptions);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async Task ShowAdoptionsByAnimal()
        {
            ShowHeader("Adoptioner efter dyr");
            Console.Write("Indtast dyrets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int animalId))
            {
                ShowError("Ugyldigt dyre ID");
                return;
            }

            var adoptions = await _adoptionService.GetAdoptionsByAnimalAsync(animalId);
            DisplayAdoptions(adoptions);
        }

        private async Task ShowAdoptionsByCustomer()
        {
            ShowHeader("Adoptioner efter kunde");
            Console.Write("Indtast kunde ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                ShowError("Ugyldigt kunde ID");
                return;
            }

            var adoptions = await _adoptionService.GetAdoptionsByCustomerAsync(customerId);
            DisplayAdoptions(adoptions);
        }

        private async Task CreateNewAdoption()
        {
            ShowHeader("Opret ny adoption");

            var animalId = ReadRequiredInt("Indtast dyrets ID");
            var customerId = ReadRequiredInt("Indtast kunde ID");
            var employeeId = ReadRequiredInt("Indtast medarbejder ID");
            var adopterName = ReadRequiredString("Indtast adoptantens navn");
            var adopterEmail = ReadRequiredString("Indtast adoptantens email");
            var adopterPhone = ReadRequiredString("Indtast adoptantens telefonnummer");
            var adoptionType = ReadRequiredString("Indtast adoptionstype");
            var notes = ReadOptionalString("Indtast noter (valgfrit)");

            var adoption = new Adoption
            {
                AnimalId = animalId,
                CustomerId = customerId,
                EmployeeId = employeeId,
                AdopterName = adopterName,
                AdopterEmail = adopterEmail,
                AdopterPhone = adopterPhone,
                AdoptionType = adoptionType,
                Notes = notes,
                Status = AdoptionStatus.Pending
            };

            try
            {
                await _adoptionService.CreateAdoptionAsync(adoption);
                ShowSuccess("Adoption oprettet succesfuldt!");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private string ReadRequiredString(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                var input = Console.ReadLine()?.Trim() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(input))
                    return input;
                ShowError("Dette felt er påkrævet. Prøv igen.");
            }
        }

        private string ReadOptionalString(string prompt)
        {
            Console.Write($"{prompt}: ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        private int ReadRequiredInt(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                if (int.TryParse(Console.ReadLine(), out int result) && result > 0)
                    return result;
                ShowError("Indtast venligst et gyldigt positivt tal. Prøv igen.");
            }
        }

        private async Task UpdateAdoption()
        {
            ShowHeader("Opdater adoption");

            Console.Write("Indtast adoption ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt adoption ID");
                return;
            }

            try
            {
                var adoption = await _adoptionService.GetAdoptionByIdAsync(id);
                if (adoption == null)
                {
                    ShowError("Adoption ikke fundet");
                    return;
                }

                if (adoption.Status == AdoptionStatus.Completed)
                {
                    ShowError("Kan ikke opdatere en gennemført adoption");
                    return;
                }

                Console.WriteLine("\nNuværende information:");
                DisplayAdoptionInfo(adoption);
                Console.WriteLine("\nIndtast ny information (tryk Enter for at beholde nuværende værdi):");

                Console.Write($"Adoptantens navn [{adoption.AdopterName}]: ");
                var adopterName = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(adopterName))
                    adoption.AdopterName = adopterName;

                Console.Write($"Adoptantens email [{adoption.AdopterEmail}]: ");
                var adopterEmail = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(adopterEmail))
                    adoption.AdopterEmail = adopterEmail;

                Console.Write($"Adoptantens telefonnummer [{adoption.AdopterPhone}]: ");
                var adopterPhone = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(adopterPhone))
                    adoption.AdopterPhone = adopterPhone;

                Console.Write($"Adoptionstype [{adoption.AdoptionType}]: ");
                var adoptionType = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(adoptionType))
                    adoption.AdoptionType = adoptionType;

                Console.Write($"Noter [{adoption.Notes}]: ");
                var notes = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(notes))
                    adoption.Notes = notes;

                await _adoptionService.UpdateAdoptionAsync(adoption);
                ShowSuccess("Adoption opdateret succesfuldt!");
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private async Task DeleteAdoption()
        {
            ShowHeader("Slet adoption");

            Console.Write("Indtast adoption ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt adoption ID");
                return;
            }

            var adoption = await _adoptionService.GetAdoptionByIdAsync(id);
            if (adoption == null)
            {
                ShowError("Adoption ikke fundet");
                return;
            }

            Console.WriteLine("\nEr du sikker på, at du vil slette denne adoption?");
            DisplayAdoptionInfo(adoption);
            Console.Write("\nSkriv 'JA' for at bekræfte: ");
            
            if (Console.ReadLine()?.ToUpper() != "JA")
            {
                Console.WriteLine("Sletning annulleret.");
                Console.WriteLine("\nTryk på en tast for at fortsætte...");
                Console.ReadKey();
                return;
            }

            await _adoptionService.DeleteAdoptionAsync(id);
            ShowSuccess("Adoption slettet succesfuldt!");
        }

        private async Task ApproveAdoption()
        {
            ShowHeader("Godkend adoption");

            Console.Write("Indtast adoption ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt adoption ID");
                return;
            }

            await _adoptionService.ApproveAdoptionAsync(id);
            ShowSuccess("Adoption godkendt succesfuldt!");
        }

        private async Task RejectAdoption()
        {
            ShowHeader("Afvis adoption");

            Console.Write("Indtast adoption ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt adoption ID");
                return;
            }

            await _adoptionService.RejectAdoptionAsync(id);
            ShowSuccess("Adoption afvist succesfuldt!");
        }

        private async Task CompleteAdoption()
        {
            ShowHeader("Gennemfør adoption");

            Console.Write("Indtast adoption ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt adoption ID");
                return;
            }

            await _adoptionService.CompleteAdoptionAsync(id);
            ShowSuccess("Adoption gennemført succesfuldt!");
        }

        private void DisplayAdoptions(IEnumerable<Adoption> adoptions)
        {
            if (!adoptions.Any())
            {
                Console.WriteLine("Ingen adoptioner fundet.");
            }
            else
            {
                foreach (var adoption in adoptions)
                {
                    DisplayAdoptionInfo(adoption);
                    Console.WriteLine(new string('-', 50));
                }
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private void DisplayAdoptionInfo(Adoption adoption)
        {
            Console.WriteLine($"ID: {adoption.Id}");
            Console.WriteLine($"Dyr ID: {adoption.AnimalId}");
            Console.WriteLine($"Kunde ID: {adoption.CustomerId}");
            Console.WriteLine($"Medarbejder ID: {adoption.EmployeeId}");
            Console.WriteLine($"Adoptant: {adoption.AdopterName}");
            Console.WriteLine($"Email: {adoption.AdopterEmail}");
            Console.WriteLine($"Telefon: {adoption.AdopterPhone}");
            Console.WriteLine($"Dato: {adoption.AdoptionDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Status: {GetStatusInDanish(adoption.Status)}");
            Console.WriteLine($"Type: {adoption.AdoptionType}");
            Console.WriteLine($"Noter: {adoption.Notes}");
            
            if (adoption.ApprovalDate.HasValue)
                Console.WriteLine($"Godkendt: {adoption.ApprovalDate.Value:dd/MM/yyyy HH:mm}");
            if (adoption.RejectionDate.HasValue)
                Console.WriteLine($"Afvist: {adoption.RejectionDate.Value:dd/MM/yyyy HH:mm}");
            if (adoption.CompletionDate.HasValue)
                Console.WriteLine($"Gennemført: {adoption.CompletionDate.Value:dd/MM/yyyy HH:mm}");
        }

        private string GetStatusInDanish(AdoptionStatus status)
        {
            return status switch
            {
                AdoptionStatus.Pending => "Under behandling",
                AdoptionStatus.Approved => "Godkendt",
                AdoptionStatus.Rejected => "Afvist",
                AdoptionStatus.Completed => "Gennemført",
                _ => status.ToString()
            };
        }
    }
} 
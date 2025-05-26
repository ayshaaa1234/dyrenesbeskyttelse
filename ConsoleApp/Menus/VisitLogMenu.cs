using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Services;
using ClassLibrary.Models;

namespace ConsoleApp.Menus
{
    /// <summary>
    /// Menu til håndtering af besøgslogge
    /// </summary>
    public class VisitLogMenu : MenuBase
    {
        private readonly VisitLogService _visitLogService;

        public VisitLogMenu(VisitLogService visitLogService)
        {
            _visitLogService = visitLogService;
        }

        public override async Task ShowAsync()
        {
            while (true)
            {
                ShowHeader("Besøgslog");
                Console.WriteLine("1. Vis alle besøgslogge");
                Console.WriteLine("2. Vis besøgslogge for dyr");
                Console.WriteLine("3. Vis besøgslogge for kunde");
                Console.WriteLine("4. Vis besøgslogge for medarbejder");
                Console.WriteLine("5. Vis besøgslogge efter dato");
                Console.WriteLine("6. Vis besøgslogge efter type");
                Console.WriteLine("7. Vis besøgslogge efter besøger");
                Console.WriteLine("8. Vis besøgslogge efter formål");
                Console.WriteLine("9. Vis besøgslogge efter varighed");
                Console.WriteLine("10. Vis lægebesøg");
                Console.WriteLine("11. Vis adoptioner");
                Console.WriteLine("12. Opret ny besøgslog");
                Console.WriteLine("13. Opdater besøgslog");
                Console.WriteLine("14. Slet besøgslog");
                Console.WriteLine("15. Marker som lægebesøg");
                Console.WriteLine("16. Marker som adoption");
                Console.WriteLine("17. Tilføj noter");
                Console.WriteLine("18. Opdater varighed");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.Write("\nVælg en mulighed: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowAllVisitLogs();
                            break;
                        case "2":
                            await ShowVisitLogsByAnimal();
                            break;
                        case "3":
                            await ShowVisitLogsByCustomer();
                            break;
                        case "4":
                            await ShowVisitLogsByEmployee();
                            break;
                        case "5":
                            await ShowVisitLogsByDate();
                            break;
                        case "6":
                            await ShowVisitLogsByType();
                            break;
                        case "7":
                            await ShowVisitLogsByVisitor();
                            break;
                        case "8":
                            await ShowVisitLogsByPurpose();
                            break;
                        case "9":
                            await ShowVisitLogsByDuration();
                            break;
                        case "10":
                            await ShowVeterinaryVisits();
                            break;
                        case "11":
                            await ShowAdoptionVisits();
                            break;
                        case "12":
                            await CreateNewVisitLog();
                            break;
                        case "13":
                            await UpdateVisitLog();
                            break;
                        case "14":
                            await DeleteVisitLog();
                            break;
                        case "15":
                            await MarkAsVeterinaryVisit();
                            break;
                        case "16":
                            await MarkAsAdoption();
                            break;
                        case "17":
                            await AddNotes();
                            break;
                        case "18":
                            await UpdateDuration();
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

        private async Task ShowAllVisitLogs()
        {
            ShowHeader("Alle besøgslogge");
            var visitLogs = await _visitLogService.GetAllVisitLogsAsync();
            DisplayVisitLogs(visitLogs);
        }

        private async Task ShowVisitLogsByAnimal()
        {
            ShowHeader("Besøgslogge for dyr");
            Console.Write("Indtast dyrets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int animalId))
            {
                ShowError("Ugyldigt dyre ID");
                return;
            }

            var visitLogs = await _visitLogService.GetVisitLogsByAnimalIdAsync(animalId);
            DisplayVisitLogs(visitLogs);
        }

        private async Task ShowVisitLogsByCustomer()
        {
            ShowHeader("Besøgslogge for kunde");
            Console.Write("Indtast kunde ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                ShowError("Ugyldigt kunde ID");
                return;
            }

            var visitLogs = await _visitLogService.GetVisitLogsByCustomerIdAsync(customerId);
            DisplayVisitLogs(visitLogs);
        }

        private async Task ShowVisitLogsByEmployee()
        {
            ShowHeader("Besøgslogge for medarbejder");
            Console.Write("Indtast medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int employeeId))
            {
                ShowError("Ugyldigt medarbejder ID");
                return;
            }

            var visitLogs = await _visitLogService.GetVisitLogsByEmployeeIdAsync(employeeId);
            DisplayVisitLogs(visitLogs);
        }

        private async Task ShowVisitLogsByDate()
        {
            ShowHeader("Besøgslogge efter dato");
            Console.Write("Indtast startdato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                ShowError("Ugyldig startdato");
                return;
            }

            Console.Write("Indtast slutdato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                ShowError("Ugyldig slutdato");
                return;
            }

            var visitLogs = await _visitLogService.GetVisitLogsByDateRangeAsync(startDate, endDate);
            DisplayVisitLogs(visitLogs);
        }

        private async Task ShowVisitLogsByType()
        {
            ShowHeader("Besøgslogge efter type");
            Console.Write("Indtast besøgstype: ");
            var visitType = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(visitType))
            {
                ShowError("Besøgstype kan ikke være tom");
                return;
            }

            var visitLogs = await _visitLogService.GetVisitLogsByVisitTypeAsync(visitType);
            DisplayVisitLogs(visitLogs);
        }

        private async Task ShowVisitLogsByVisitor()
        {
            ShowHeader("Besøgslogge efter besøger");
            Console.Write("Indtast besøger: ");
            var visitor = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(visitor))
            {
                ShowError("Besøger kan ikke være tom");
                return;
            }

            var visitLogs = await _visitLogService.GetVisitLogsByVisitorAsync(visitor);
            DisplayVisitLogs(visitLogs);
        }

        private async Task ShowVisitLogsByPurpose()
        {
            ShowHeader("Besøgslogge efter formål");
            Console.Write("Indtast formål: ");
            var purpose = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(purpose))
            {
                ShowError("Formål kan ikke være tomt");
                return;
            }

            var visitLogs = await _visitLogService.GetVisitLogsByPurposeAsync(purpose);
            DisplayVisitLogs(visitLogs);
        }

        private async Task ShowVisitLogsByDuration()
        {
            ShowHeader("Besøgslogge efter varighed");
            Console.Write("Indtast minimumsvarighed (minutter): ");
            if (!int.TryParse(Console.ReadLine(), out int minDuration))
            {
                ShowError("Ugyldig minimumsvarighed");
                return;
            }

            Console.Write("Indtast maksimumsvarighed (minutter): ");
            if (!int.TryParse(Console.ReadLine(), out int maxDuration))
            {
                ShowError("Ugyldig maksimumsvarighed");
                return;
            }

            var visitLogs = await _visitLogService.GetVisitLogsByDurationRangeAsync(minDuration, maxDuration);
            DisplayVisitLogs(visitLogs);
        }

        private async Task ShowVeterinaryVisits()
        {
            ShowHeader("Lægebesøg");
            var visitLogs = await _visitLogService.GetVeterinaryVisitsAsync();
            DisplayVisitLogs(visitLogs);
        }

        private async Task ShowAdoptionVisits()
        {
            ShowHeader("Adoptioner");
            var visitLogs = await _visitLogService.GetVisitsResultingInAdoptionAsync();
            DisplayVisitLogs(visitLogs);
        }

        private async Task CreateNewVisitLog()
        {
            ShowHeader("Opret ny besøgslog");

            Console.Write("Indtast dyrets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int animalId))
            {
                ShowError("Ugyldigt dyre ID");
                return;
            }

            Console.Write("Indtast kunde ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                ShowError("Ugyldigt kunde ID");
                return;
            }

            Console.Write("Indtast medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int employeeId))
            {
                ShowError("Ugyldigt medarbejder ID");
                return;
            }

            Console.Write("Indtast besøgstype: ");
            var visitType = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast besøger: ");
            var visitor = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast formål: ");
            var purpose = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast varighed (minutter): ");
            if (!int.TryParse(Console.ReadLine(), out int duration))
            {
                ShowError("Ugyldig varighed");
                return;
            }

            Console.Write("Indtast beskrivelse: ");
            var description = Console.ReadLine() ?? string.Empty;

            var visitLog = new VisitLog
            {
                AnimalId = animalId,
                CustomerId = customerId,
                EmployeeId = employeeId,
                VisitType = visitType,
                Visitor = visitor,
                Purpose = purpose,
                Duration = duration,
                Description = description,
                VisitDate = DateTime.Now
            };

            await _visitLogService.CreateVisitLogAsync(visitLog);
            ShowSuccess("Besøgslog oprettet succesfuldt!");
        }

        private async Task UpdateVisitLog()
        {
            ShowHeader("Opdater besøgslog");

            Console.Write("Indtast besøgslog ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var visitLog = await _visitLogService.GetVisitLogByIdAsync(id);
            if (visitLog == null)
            {
                ShowError("Besøgslog ikke fundet");
                return;
            }

            Console.WriteLine("\nNuværende information:");
            DisplayVisitLogInfo(visitLog);
            Console.WriteLine("\nIndtast ny information (tryk Enter for at beholde nuværende værdi):");

            Console.Write($"Besøgstype [{visitLog.VisitType}]: ");
            var visitType = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(visitType))
                visitLog.VisitType = visitType;

            Console.Write($"Besøger [{visitLog.Visitor}]: ");
            var visitor = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(visitor))
                visitLog.Visitor = visitor;

            Console.Write($"Formål [{visitLog.Purpose}]: ");
            var purpose = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(purpose))
                visitLog.Purpose = purpose;

            Console.Write($"Varighed [{visitLog.Duration}]: ");
            var durationStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(durationStr) && int.TryParse(durationStr, out int duration))
                visitLog.Duration = duration;

            Console.Write($"Beskrivelse [{visitLog.Description}]: ");
            var description = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(description))
                visitLog.Description = description;

            await _visitLogService.UpdateVisitLogAsync(visitLog);
            ShowSuccess("Besøgslog opdateret succesfuldt!");
        }

        private async Task DeleteVisitLog()
        {
            ShowHeader("Slet besøgslog");

            Console.Write("Indtast besøgslog ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var visitLog = await _visitLogService.GetVisitLogByIdAsync(id);
            if (visitLog == null)
            {
                ShowError("Besøgslog ikke fundet");
                return;
            }

            Console.WriteLine("\nEr du sikker på, at du vil slette denne besøgslog?");
            DisplayVisitLogInfo(visitLog);
            Console.Write("\nSkriv 'JA' for at bekræfte: ");
            
            if (Console.ReadLine()?.ToUpper() != "JA")
            {
                Console.WriteLine("Sletning annulleret.");
                Console.WriteLine("\nTryk på en tast for at fortsætte...");
                Console.ReadKey();
                return;
            }

            await _visitLogService.DeleteVisitLogAsync(id);
            ShowSuccess("Besøgslog slettet succesfuldt!");
        }

        private async Task MarkAsVeterinaryVisit()
        {
            ShowHeader("Marker som lægebesøg");

            Console.Write("Indtast besøgslog ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _visitLogService.MarkAsVeterinaryVisitAsync(id);
            ShowSuccess("Besøgslog markeret som lægebesøg!");
        }

        private async Task MarkAsAdoption()
        {
            ShowHeader("Marker som adoption");

            Console.Write("Indtast besøgslog ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _visitLogService.MarkAsResultedInAdoptionAsync(id);
            ShowSuccess("Besøgslog markeret som adoption!");
        }

        private async Task AddNotes()
        {
            ShowHeader("Tilføj noter");

            Console.Write("Indtast besøgslog ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast noter: ");
            var notes = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(notes))
            {
                ShowError("Noter kan ikke være tomme");
                return;
            }

            await _visitLogService.AddNotesAsync(id, notes);
            ShowSuccess("Noter tilføjet succesfuldt!");
        }

        private async Task UpdateDuration()
        {
            ShowHeader("Opdater varighed");

            Console.Write("Indtast besøgslog ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            Console.Write("Indtast ny varighed (minutter): ");
            if (!int.TryParse(Console.ReadLine(), out int duration))
            {
                ShowError("Ugyldig varighed");
                return;
            }

            await _visitLogService.UpdateDurationAsync(id, duration);
            ShowSuccess("Varighed opdateret succesfuldt!");
        }

        private void DisplayVisitLogs(IEnumerable<VisitLog> visitLogs)
        {
            if (!visitLogs.Any())
            {
                Console.WriteLine("Ingen besøgslogge fundet.");
            }
            else
            {
                foreach (var visitLog in visitLogs)
                {
                    DisplayVisitLogInfo(visitLog);
                    Console.WriteLine(new string('-', 50));
                }
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private void DisplayVisitLogInfo(VisitLog visitLog)
        {
            Console.WriteLine($"ID: {visitLog.Id}");
            Console.WriteLine($"Dyr ID: {visitLog.AnimalId}");
            Console.WriteLine($"Kunde ID: {visitLog.CustomerId}");
            Console.WriteLine($"Medarbejder ID: {visitLog.EmployeeId}");
            Console.WriteLine($"Dato: {visitLog.VisitDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Type: {visitLog.VisitType}");
            Console.WriteLine($"Besøger: {visitLog.Visitor}");
            Console.WriteLine($"Formål: {visitLog.Purpose}");
            Console.WriteLine($"Varighed: {visitLog.Duration} minutter");
            Console.WriteLine($"Beskrivelse: {visitLog.Description}");
            Console.WriteLine($"Lægebesøg: {(visitLog.IsVeterinaryVisit ? "Ja" : "Nej")}");
            Console.WriteLine($"Adoption: {(visitLog.ResultedInAdoption ? "Ja" : "Nej")}");
            Console.WriteLine($"Noter: {visitLog.Notes}");
        }
    }
}

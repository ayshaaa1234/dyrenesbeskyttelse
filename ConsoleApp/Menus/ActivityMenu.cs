using System;
using System.Threading.Tasks;
using ClassLibrary.Services;
using ClassLibrary.Models;

namespace ConsoleApp.Menus
{
    /// <summary>
    /// Menu til håndtering af aktiviteter
    /// </summary>
    public class ActivityMenu : MenuBase
    {
        private readonly ActivityService _activityService;

        public ActivityMenu(ActivityService activityService)
        {
            _activityService = activityService;
        }

        public override async Task ShowAsync()
        {
            while (true)
            {
                ShowHeader("Aktiviteter");
                Console.WriteLine("1. Vis alle aktiviteter");
                Console.WriteLine("2. Vis kommende aktiviteter");
                Console.WriteLine("3. Vis aktiviteter med ledige pladser");
                Console.WriteLine("4. Tilføj ny aktivitet");
                Console.WriteLine("5. Opdater aktivitet");
                Console.WriteLine("6. Slet aktivitet");
                Console.WriteLine("7. Søg efter aktiviteter");
                Console.WriteLine("8. Håndter deltagere");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.Write("\nVælg en mulighed: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowAllActivities();
                            break;
                        case "2":
                            await ShowUpcomingActivities();
                            break;
                        case "3":
                            await ShowActivitiesWithAvailableSpots();
                            break;
                        case "4":
                            await AddNewActivity();
                            break;
                        case "5":
                            await UpdateActivity();
                            break;
                        case "6":
                            await DeleteActivity();
                            break;
                        case "7":
                            await SearchActivities();
                            break;
                        case "8":
                            await ManageParticipants();
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

        private async Task ShowAllActivities()
        {
            ShowHeader("Alle aktiviteter");
            var activities = await _activityService.GetAllActivitiesAsync();
            DisplayActivities(activities);
        }

        private async Task ShowUpcomingActivities()
        {
            ShowHeader("Kommende aktiviteter");
            var activities = await _activityService.GetUpcomingActivitiesAsync();
            DisplayActivities(activities);
        }

        private async Task ShowActivitiesWithAvailableSpots()
        {
            ShowHeader("Aktiviteter med ledige pladser");
            var activities = await _activityService.GetActivitiesWithAvailableSpotsAsync();
            DisplayActivities(activities);
        }

        private async Task AddNewActivity()
        {
            ShowHeader("Tilføj ny aktivitet");

            Console.Write("Navn: ");
            var name = Console.ReadLine() ?? string.Empty;

            Console.Write("Beskrivelse: ");
            var description = Console.ReadLine() ?? string.Empty;

            Console.Write("Dato (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
            {
                ShowError("Ugyldig dato");
                return;
            }

            Console.Write("Varighed (minutter): ");
            if (!int.TryParse(Console.ReadLine(), out int duration))
            {
                ShowError("Ugyldig varighed");
                return;
            }

            Console.Write("Maksimalt antal deltagere: ");
            if (!int.TryParse(Console.ReadLine(), out int maxParticipants))
            {
                ShowError("Ugyldigt antal deltagere");
                return;
            }

            Console.Write("Medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int employeeId))
            {
                ShowError("Ugyldigt medarbejder ID");
                return;
            }

            Console.Write("Lokation: ");
            var location = Console.ReadLine() ?? string.Empty;

            Console.Write("Pris: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                ShowError("Ugyldig pris");
                return;
            }

            var activity = new Activity
            {
                Name = name,
                Description = description,
                ActivityDate = date,
                DurationMinutes = duration,
                MaxParticipants = maxParticipants,
                EmployeeId = employeeId,
                Location = location,
                Price = price,
                Status = ActivityStatus.Planned
            };

            await _activityService.CreateActivityAsync(activity);
            ShowSuccess("Aktivitet tilføjet succesfuldt!");
        }

        private async Task UpdateActivity()
        {
            ShowHeader("Opdater aktivitet");

            Console.Write("Indtast aktivitets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null)
            {
                ShowError("Aktivitet ikke fundet");
                return;
            }

            Console.WriteLine("\nNuværende information:");
            DisplayActivityInfo(activity);
            Console.WriteLine("\nIndtast ny information (tryk Enter for at beholde nuværende værdi):");

            Console.Write($"Navn [{activity.Name}]: ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
                activity.Name = name;

            Console.Write($"Beskrivelse [{activity.Description}]: ");
            var description = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(description))
                activity.Description = description;

            Console.Write($"Dato [{activity.ActivityDate:dd/MM/yyyy}]: ");
            var dateStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dateStr) && DateTime.TryParse(dateStr, out DateTime date))
                activity.ActivityDate = date;

            Console.Write($"Varighed [{activity.DurationMinutes}]: ");
            var durationStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(durationStr) && int.TryParse(durationStr, out int duration))
                activity.DurationMinutes = duration;

            Console.Write($"Maksimalt antal deltagere [{activity.MaxParticipants}]: ");
            var maxParticipantsStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(maxParticipantsStr) && int.TryParse(maxParticipantsStr, out int maxParticipants))
                activity.MaxParticipants = maxParticipants;

            Console.Write($"Medarbejder ID [{activity.EmployeeId}]: ");
            var employeeIdStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(employeeIdStr) && int.TryParse(employeeIdStr, out int employeeId))
                activity.EmployeeId = employeeId;

            Console.Write($"Lokation [{activity.Location}]: ");
            var location = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(location))
                activity.Location = location;

            Console.Write($"Pris [{activity.Price}]: ");
            var priceStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(priceStr) && decimal.TryParse(priceStr, out decimal price))
                activity.Price = price;

            Console.WriteLine("\nVælg status:");
            Console.WriteLine("1. Planlagt");
            Console.WriteLine("2. I gang");
            Console.WriteLine("3. Afsluttet");
            Console.WriteLine("4. Aflyst");
            Console.Write("Valg: ");

            var statusChoice = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(statusChoice))
            {
                switch (statusChoice)
                {
                    case "1": activity.Status = ActivityStatus.Planned; break;
                    case "2": activity.Status = ActivityStatus.InProgress; break;
                    case "3": activity.Status = ActivityStatus.Completed; break;
                    case "4": activity.Status = ActivityStatus.Cancelled; break;
                    default:
                        ShowError("Ugyldigt valg");
                        return;
                }
            }

            await _activityService.UpdateActivityAsync(activity);
            ShowSuccess("Aktivitet opdateret succesfuldt!");
        }

        private async Task DeleteActivity()
        {
            ShowHeader("Slet aktivitet");

            Console.Write("Indtast aktivitets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null)
            {
                ShowError("Aktivitet ikke fundet");
                return;
            }

            Console.WriteLine("\nEr du sikker på, at du vil slette denne aktivitet?");
            DisplayActivityInfo(activity);
            Console.Write("\nSkriv 'JA' for at bekræfte: ");
            
            if (Console.ReadLine()?.ToUpper() != "JA")
            {
                Console.WriteLine("Sletning annulleret.");
                Console.WriteLine("\nTryk på en tast for at fortsætte...");
                Console.ReadKey();
                return;
            }

            await _activityService.DeleteActivityAsync(id);
            ShowSuccess("Aktivitet slettet succesfuldt!");
        }

        private async Task SearchActivities()
        {
            while (true)
            {
                ShowHeader("Søg efter aktiviteter");
                Console.WriteLine("1. Søg efter dato");
                Console.WriteLine("2. Søg efter status");
                Console.WriteLine("3. Søg efter lokation");
                Console.WriteLine("4. Søg efter pris");
                Console.WriteLine("0. Tilbage");

                Console.Write("\nVælg en mulighed: ");
                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await SearchByDate();
                            break;
                        case "2":
                            await SearchByStatus();
                            break;
                        case "3":
                            await SearchByLocation();
                            break;
                        case "4":
                            await SearchByPrice();
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

        private async Task SearchByDate()
        {
            ShowHeader("Søg efter dato");
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

            var activities = await _activityService.GetActivitiesByDateRangeAsync(startDate, endDate);
            DisplayActivities(activities);
        }

        private async Task SearchByStatus()
        {
            ShowHeader("Søg efter status");
            Console.WriteLine("Vælg status:");
            Console.WriteLine("1. Planlagt");
            Console.WriteLine("2. I gang");
            Console.WriteLine("3. Afsluttet");
            Console.WriteLine("4. Aflyst");
            Console.Write("Valg: ");

            ActivityStatus status;
            switch (Console.ReadLine())
            {
                case "1": status = ActivityStatus.Planned; break;
                case "2": status = ActivityStatus.InProgress; break;
                case "3": status = ActivityStatus.Completed; break;
                case "4": status = ActivityStatus.Cancelled; break;
                default:
                    ShowError("Ugyldigt valg");
                    return;
            }

            var activities = await _activityService.GetActivitiesByStatusAsync(status);
            DisplayActivities(activities);
        }

        private async Task SearchByLocation()
        {
            ShowHeader("Søg efter lokation");
            Console.Write("Indtast lokation: ");
            var location = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(location))
            {
                ShowError("Lokation kan ikke være tom");
                return;
            }

            var activities = await _activityService.GetActivitiesByLocationAsync(location);
            DisplayActivities(activities);
        }

        private async Task SearchByPrice()
        {
            ShowHeader("Søg efter pris");
            Console.Write("Indtast minimumspris: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal minPrice))
            {
                ShowError("Ugyldig minimumspris");
                return;
            }

            Console.Write("Indtast maksimumspris: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal maxPrice))
            {
                ShowError("Ugyldig maksimumspris");
                return;
            }

            var activities = await _activityService.GetActivitiesByPriceRangeAsync(minPrice, maxPrice);
            DisplayActivities(activities);
        }

        private async Task ManageParticipants()
        {
            while (true)
            {
                ShowHeader("Håndter deltagere");
                Console.WriteLine("1. Tilføj deltager");
                Console.WriteLine("2. Fjern deltager");
                Console.WriteLine("3. Tjek deltagerstatus");
                Console.WriteLine("0. Tilbage");

                Console.Write("\nVælg en mulighed: ");
                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await AddParticipant();
                            break;
                        case "2":
                            await RemoveParticipant();
                            break;
                        case "3":
                            await CheckParticipantStatus();
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

        private async Task AddParticipant()
        {
            ShowHeader("Tilføj deltager");

            Console.Write("Indtast aktivitets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int activityId))
            {
                ShowError("Ugyldigt aktivitets ID");
                return;
            }

            Console.Write("Indtast deltager ID: ");
            if (!int.TryParse(Console.ReadLine(), out int participantId))
            {
                ShowError("Ugyldigt deltager ID");
                return;
            }

            await _activityService.AddParticipantToActivityAsync(activityId, participantId);
            ShowSuccess("Deltager tilføjet succesfuldt!");
        }

        private async Task RemoveParticipant()
        {
            ShowHeader("Fjern deltager");

            Console.Write("Indtast aktivitets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int activityId))
            {
                ShowError("Ugyldigt aktivitets ID");
                return;
            }

            Console.Write("Indtast deltager ID: ");
            if (!int.TryParse(Console.ReadLine(), out int participantId))
            {
                ShowError("Ugyldigt deltager ID");
                return;
            }

            await _activityService.RemoveParticipantFromActivityAsync(activityId, participantId);
            ShowSuccess("Deltager fjernet succesfuldt!");
        }

        private async Task CheckParticipantStatus()
        {
            ShowHeader("Tjek deltagerstatus");

            Console.Write("Indtast aktivitets ID: ");
            if (!int.TryParse(Console.ReadLine(), out int activityId))
            {
                ShowError("Ugyldigt aktivitets ID");
                return;
            }

            Console.Write("Indtast deltager ID: ");
            if (!int.TryParse(Console.ReadLine(), out int participantId))
            {
                ShowError("Ugyldigt deltager ID");
                return;
            }

            var isRegistered = await _activityService.IsParticipantRegisteredAsync(activityId, participantId);
            Console.WriteLine($"\nDeltager er {(isRegistered ? "tilmeldt" : "ikke tilmeldt")} aktiviteten.");
            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private void DisplayActivities(IEnumerable<Activity> activities)
        {
            if (!activities.Any())
            {
                Console.WriteLine("Ingen aktiviteter fundet.");
            }
            else
            {
                foreach (var activity in activities)
                {
                    DisplayActivityInfo(activity);
                    Console.WriteLine(new string('-', 50));
                }
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private void DisplayActivityInfo(Activity activity)
        {
            Console.WriteLine($"ID: {activity.Id}");
            Console.WriteLine($"Navn: {activity.Name}");
            Console.WriteLine($"Beskrivelse: {activity.Description}");
            Console.WriteLine($"Dato: {activity.ActivityDate:dd/MM/yyyy}");
            Console.WriteLine($"Varighed: {activity.DurationMinutes} minutter");
            Console.WriteLine($"Maksimalt antal deltagere: {activity.MaxParticipants}");
            Console.WriteLine($"Antal tilmeldte: {activity.ParticipantIds.Count}");
            Console.WriteLine($"Ledige pladser: {activity.AvailableSpots}");
            Console.WriteLine($"Medarbejder ID: {activity.EmployeeId}");
            Console.WriteLine($"Lokation: {activity.Location}");
            Console.WriteLine($"Pris: {activity.Price:C}");
            Console.WriteLine($"Status: {activity.Status}");
        }
    }
} 
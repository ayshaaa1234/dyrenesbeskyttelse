using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary.Services;
using ClassLibrary.Models;

namespace ConsoleApp.Menus
{
    /// <summary>
    /// Menu til håndtering af bookinger
    /// </summary>
    public class BookingMenu : MenuBase
    {
        private readonly BookingService _bookingService;

        public BookingMenu(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public override async Task ShowAsync()
        {
            while (true)
            {
                ShowHeader("Booking");
                Console.WriteLine("1. Vis alle bookinger");
                Console.WriteLine("2. Vis booking efter ID");
                Console.WriteLine("3. Vis bookinger for dyr");
                Console.WriteLine("4. Vis bookinger for kunde");
                Console.WriteLine("5. Vis bookinger for medarbejder");
                Console.WriteLine("6. Vis bookinger efter dato");
                Console.WriteLine("7. Vis bookinger efter status");
                Console.WriteLine("8. Vis bookinger efter type");
                Console.WriteLine("9. Vis kommende bookinger");
                Console.WriteLine("10. Vis aflyste bookinger");
                Console.WriteLine("11. Vis gennemførte bookinger");
                Console.WriteLine("12. Vis seneste booking for dyr");
                Console.WriteLine("13. Vis seneste booking for kunde");
                Console.WriteLine("14. Opret ny booking");
                Console.WriteLine("15. Opdater booking");
                Console.WriteLine("16. Slet booking");
                Console.WriteLine("17. Bekræft booking");
                Console.WriteLine("18. Aflys booking");
                Console.WriteLine("19. Marker booking som gennemført");
                Console.WriteLine("20. Tilføj booking til venteliste");
                Console.WriteLine("0. Tilbage til hovedmenu");
                Console.Write("\nVælg en mulighed: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowAllBookings();
                            break;
                        case "2":
                            await ShowBookingById();
                            break;
                        case "3":
                            await ShowBookingsByAnimal();
                            break;
                        case "4":
                            await ShowBookingsByCustomer();
                            break;
                        case "5":
                            await ShowBookingsByEmployee();
                            break;
                        case "6":
                            await ShowBookingsByDate();
                            break;
                        case "7":
                            await ShowBookingsByStatus();
                            break;
                        case "8":
                            await ShowBookingsByType();
                            break;
                        case "9":
                            await ShowUpcomingBookings();
                            break;
                        case "10":
                            await ShowCancelledBookings();
                            break;
                        case "11":
                            await ShowCompletedBookings();
                            break;
                        case "12":
                            await ShowLatestBookingForAnimal();
                            break;
                        case "13":
                            await ShowLatestBookingForCustomer();
                            break;
                        case "14":
                            await CreateNewBooking();
                            break;
                        case "15":
                            await UpdateBooking();
                            break;
                        case "16":
                            await DeleteBooking();
                            break;
                        case "17":
                            await ConfirmBooking();
                            break;
                        case "18":
                            await CancelBooking();
                            break;
                        case "19":
                            await CompleteBooking();
                            break;
                        case "20":
                            await WaitlistBooking();
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

        private async Task ShowAllBookings()
        {
            ShowHeader("Alle bookinger");
            var bookings = await _bookingService.GetAllBookingsAsync();
            DisplayBookings(bookings);
        }

        private async Task ShowBookingById()
        {
            ShowHeader("Booking efter ID");
            Console.Write("Indtast booking ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            DisplayBookingInfo(booking);
        }

        private async Task ShowBookingsByAnimal()
        {
            ShowHeader("Bookinger for dyr");
            Console.Write("Indtast dyr ID: ");
            if (!int.TryParse(Console.ReadLine(), out int animalId))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var bookings = await _bookingService.GetBookingsByAnimalAsync(animalId);
            DisplayBookings(bookings);
        }

        private async Task ShowBookingsByCustomer()
        {
            ShowHeader("Bookinger for kunde");
            Console.Write("Indtast kunde ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var bookings = await _bookingService.GetBookingsByCustomerAsync(customerId);
            DisplayBookings(bookings);
        }

        private async Task ShowBookingsByEmployee()
        {
            ShowHeader("Bookinger for medarbejder");
            Console.Write("Indtast medarbejder ID: ");
            if (!int.TryParse(Console.ReadLine(), out int employeeId))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var bookings = await _bookingService.GetBookingsByEmployeeAsync(employeeId);
            DisplayBookings(bookings);
        }

        private async Task ShowBookingsByDate()
        {
            ShowHeader("Bookinger efter dato");
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

            var bookings = await _bookingService.GetBookingsByDateRangeAsync(startDate, endDate);
            DisplayBookings(bookings);
        }

        private async Task ShowBookingsByStatus()
        {
            ShowHeader("Bookinger efter status");
            Console.WriteLine("Vælg status:");
            Console.WriteLine("1. Bekræftet");
            Console.WriteLine("2. Aflyst");
            Console.WriteLine("3. Gennemført");
            Console.WriteLine("4. På venteliste");
            Console.Write("\nVælg en mulighed: ");

            var statusChoice = Console.ReadLine();
            string status = statusChoice switch
            {
                "1" => "Confirmed",
                "2" => "Cancelled",
                "3" => "Completed",
                "4" => "Waitlisted",
                _ => throw new ArgumentException("Ugyldigt valg")
            };

            var bookings = await _bookingService.GetBookingsByStatusAsync(status);
            DisplayBookings(bookings);
        }

        private async Task ShowBookingsByType()
        {
            ShowHeader("Bookinger efter type");
            Console.Write("Indtast type: ");
            var type = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(type))
            {
                ShowError("Type kan ikke være tom");
                return;
            }

            var bookings = await _bookingService.GetBookingsByTypeAsync(type);
            DisplayBookings(bookings);
        }

        private async Task ShowUpcomingBookings()
        {
            ShowHeader("Kommende bookinger");
            var bookings = await _bookingService.GetUpcomingBookingsAsync();
            DisplayBookings(bookings);
        }

        private async Task ShowCancelledBookings()
        {
            ShowHeader("Aflyste bookinger");
            var bookings = await _bookingService.GetCancelledBookingsAsync();
            DisplayBookings(bookings);
        }

        private async Task ShowCompletedBookings()
        {
            ShowHeader("Gennemførte bookinger");
            var bookings = await _bookingService.GetCompletedBookingsAsync();
            DisplayBookings(bookings);
        }

        private async Task ShowLatestBookingForAnimal()
        {
            ShowHeader("Seneste booking for dyr");
            Console.Write("Indtast dyr ID: ");
            if (!int.TryParse(Console.ReadLine(), out int animalId))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var booking = await _bookingService.GetLatestBookingForAnimalAsync(animalId);
            DisplayBookingInfo(booking);
        }

        private async Task ShowLatestBookingForCustomer()
        {
            ShowHeader("Seneste booking for kunde");
            Console.Write("Indtast kunde ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var booking = await _bookingService.GetLatestBookingForCustomerAsync(customerId);
            DisplayBookingInfo(booking);
        }

        private async Task CreateNewBooking()
        {
            ShowHeader("Opret ny booking");

            Console.Write("Indtast dyr ID: ");
            if (!int.TryParse(Console.ReadLine(), out int animalId))
            {
                ShowError("Ugyldigt dyr ID");
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

            Console.Write("Indtast dato og tid (dd/mm/yyyy HH:mm): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime bookingDate))
            {
                ShowError("Ugyldig dato");
                return;
            }

            Console.Write("Indtast varighed i minutter: ");
            if (!int.TryParse(Console.ReadLine(), out int durationMinutes))
            {
                ShowError("Ugyldig varighed");
                return;
            }

            Console.Write("Indtast type: ");
            var type = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast formål: ");
            var purpose = Console.ReadLine() ?? string.Empty;

            Console.Write("Indtast noter: ");
            var notes = Console.ReadLine() ?? string.Empty;

            var booking = new Booking
            {
                AnimalId = animalId,
                CustomerId = customerId,
                EmployeeId = employeeId,
                BookingDate = bookingDate,
                DurationMinutes = durationMinutes,
                Type = type,
                Purpose = purpose,
                Notes = notes,
                Status = Booking.BookingStatus.Confirmed
            };

            await _bookingService.CreateBookingAsync(booking);
            ShowSuccess("Booking oprettet succesfuldt!");
        }

        private async Task UpdateBooking()
        {
            ShowHeader("Opdater booking");

            Console.Write("Indtast booking ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                ShowError("Booking ikke fundet");
                return;
            }

            Console.WriteLine("\nNuværende information:");
            DisplayBookingInfo(booking);
            Console.WriteLine("\nIndtast ny information (tryk Enter for at beholde nuværende værdi):");

            Console.Write($"Dyr ID [{booking.AnimalId}]: ");
            var animalIdStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(animalIdStr) && int.TryParse(animalIdStr, out int animalId))
                booking.AnimalId = animalId;

            Console.Write($"Kunde ID [{booking.CustomerId}]: ");
            var customerIdStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(customerIdStr) && int.TryParse(customerIdStr, out int customerId))
                booking.CustomerId = customerId;

            Console.Write($"Medarbejder ID [{booking.EmployeeId}]: ");
            var employeeIdStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(employeeIdStr) && int.TryParse(employeeIdStr, out int employeeId))
                booking.EmployeeId = employeeId;

            Console.Write($"Dato og tid [{booking.BookingDate:dd/MM/yyyy HH:mm}]: ");
            var dateStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dateStr) && DateTime.TryParse(dateStr, out DateTime bookingDate))
                booking.BookingDate = bookingDate;

            Console.Write($"Varighed i minutter [{booking.DurationMinutes}]: ");
            var durationStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(durationStr) && int.TryParse(durationStr, out int durationMinutes))
                booking.DurationMinutes = durationMinutes;

            Console.Write($"Type [{booking.Type}]: ");
            var type = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(type))
                booking.Type = type;

            Console.Write($"Formål [{booking.Purpose}]: ");
            var purpose = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(purpose))
                booking.Purpose = purpose;

            Console.Write($"Noter [{booking.Notes}]: ");
            var notes = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(notes))
                booking.Notes = notes;

            await _bookingService.UpdateBookingAsync(booking);
            ShowSuccess("Booking opdateret succesfuldt!");
        }

        private async Task DeleteBooking()
        {
            ShowHeader("Slet booking");

            Console.Write("Indtast booking ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                ShowError("Booking ikke fundet");
                return;
            }

            Console.WriteLine("\nEr du sikker på, at du vil slette denne booking?");
            DisplayBookingInfo(booking);
            Console.Write("\nSkriv 'JA' for at bekræfte: ");
            
            if (Console.ReadLine()?.ToUpper() != "JA")
            {
                Console.WriteLine("Sletning annulleret.");
                Console.WriteLine("\nTryk på en tast for at fortsætte...");
                Console.ReadKey();
                return;
            }

            await _bookingService.DeleteBookingAsync(id);
            ShowSuccess("Booking slettet succesfuldt!");
        }

        private async Task ConfirmBooking()
        {
            ShowHeader("Bekræft booking");

            Console.Write("Indtast booking ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _bookingService.ConfirmBookingAsync(id);
            ShowSuccess("Booking bekræftet succesfuldt!");
        }

        private async Task CancelBooking()
        {
            ShowHeader("Aflys booking");

            Console.Write("Indtast booking ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _bookingService.CancelBookingAsync(id);
            ShowSuccess("Booking aflyst succesfuldt!");
        }

        private async Task CompleteBooking()
        {
            ShowHeader("Marker booking som gennemført");

            Console.Write("Indtast booking ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _bookingService.CompleteBookingAsync(id);
            ShowSuccess("Booking markeret som gennemført!");
        }

        private async Task WaitlistBooking()
        {
            ShowHeader("Tilføj booking til venteliste");

            Console.Write("Indtast booking ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Ugyldigt ID");
                return;
            }

            await _bookingService.WaitlistBookingAsync(id);
            ShowSuccess("Booking tilføjet til venteliste!");
        }

        private void DisplayBookings(IEnumerable<Booking> bookings)
        {
            if (!bookings.Any())
            {
                Console.WriteLine("Ingen bookinger fundet.");
            }
            else
            {
                foreach (var booking in bookings)
                {
                    DisplayBookingInfo(booking);
                    Console.WriteLine(new string('-', 50));
                }
            }

            Console.WriteLine("\nTryk på en tast for at fortsætte...");
            Console.ReadKey();
        }

        private void DisplayBookingInfo(Booking booking)
        {
            Console.WriteLine($"ID: {booking.Id}");
            Console.WriteLine($"Dyr ID: {booking.AnimalId}");
            Console.WriteLine($"Kunde ID: {booking.CustomerId}");
            Console.WriteLine($"Medarbejder ID: {booking.EmployeeId}");
            Console.WriteLine($"Dato og tid: {booking.BookingDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Varighed: {booking.DurationMinutes} minutter");
            Console.WriteLine($"Type: {booking.Type}");
            Console.WriteLine($"Formål: {booking.Purpose}");
            Console.WriteLine($"Status: {booking.Status}");
            if (!string.IsNullOrWhiteSpace(booking.Notes))
            {
                Console.WriteLine($"Noter: {booking.Notes}");
            }
        }
    }
}

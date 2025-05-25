using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for service til håndtering af bookinger
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Henter alle bookinger
        /// </summary>
        Task<IEnumerable<Booking>> GetAllBookingsAsync();

        /// <summary>
        /// Henter en booking baseret på ID
        /// </summary>
        Task<Booking> GetBookingByIdAsync(int id);

        /// <summary>
        /// Opretter en ny booking
        /// </summary>
        Task<Booking> CreateBookingAsync(Booking booking);

        /// <summary>
        /// Opdaterer en eksisterende booking
        /// </summary>
        Task<Booking> UpdateBookingAsync(Booking booking);

        /// <summary>
        /// Sletter en booking
        /// </summary>
        Task DeleteBookingAsync(int id);

        /// <summary>
        /// Henter bookinger for et bestemt dyr
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingsByAnimalAsync(int animalId);

        /// <summary>
        /// Henter bookinger for en bestemt kunde
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingsByCustomerAsync(int customerId);

        /// <summary>
        /// Henter bookinger for en bestemt medarbejder
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingsByEmployeeAsync(int employeeId);

        /// <summary>
        /// Henter bookinger i et datointerval
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter bookinger baseret på status
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingsByStatusAsync(string status);

        /// <summary>
        /// Henter bookinger baseret på type
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingsByTypeAsync(string type);

        /// <summary>
        /// Henter kommende bookinger
        /// </summary>
        Task<IEnumerable<Booking>> GetUpcomingBookingsAsync();

        /// <summary>
        /// Henter aflyste bookinger
        /// </summary>
        Task<IEnumerable<Booking>> GetCancelledBookingsAsync();

        /// <summary>
        /// Henter gennemførte bookinger
        /// </summary>
        Task<IEnumerable<Booking>> GetCompletedBookingsAsync();

        /// <summary>
        /// Henter den seneste booking for et dyr
        /// </summary>
        Task<Booking> GetLatestBookingForAnimalAsync(int animalId);

        /// <summary>
        /// Henter den seneste booking for en kunde
        /// </summary>
        Task<Booking> GetLatestBookingForCustomerAsync(int customerId);

        /// <summary>
        /// Bekræfter en booking
        /// </summary>
        Task ConfirmBookingAsync(int id);

        /// <summary>
        /// Aflyser en booking
        /// </summary>
        Task CancelBookingAsync(int id);

        /// <summary>
        /// Marker en booking som gennemført
        /// </summary>
        Task CompleteBookingAsync(int id);

        /// <summary>
        /// Tilføjer en booking til venteliste
        /// </summary>
        Task WaitlistBookingAsync(int id);
    }
} 
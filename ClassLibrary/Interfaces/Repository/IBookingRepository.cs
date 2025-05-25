using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for repository til håndtering af bookinger
    /// </summary>
    public interface IBookingRepository : IRepository<Booking>
    {
        /// <summary>
        /// Finder bookinger for et bestemt dyr
        /// </summary>
        Task<IEnumerable<Booking>> GetByAnimalIdAsync(int animalId);

        /// <summary>
        /// Finder bookinger for en bestemt kunde
        /// </summary>
        Task<IEnumerable<Booking>> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// Finder bookinger for en bestemt medarbejder
        /// </summary>
        Task<IEnumerable<Booking>> GetByEmployeeIdAsync(int employeeId);

        /// <summary>
        /// Finder bookinger baseret på datointerval
        /// </summary>
        Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder bookinger baseret på status
        /// </summary>
        Task<IEnumerable<Booking>> GetByStatusAsync(string status);

        /// <summary>
        /// Finder bookinger baseret på type
        /// </summary>
        Task<IEnumerable<Booking>> GetByTypeAsync(string type);

        /// <summary>
        /// Finder kommende bookinger
        /// </summary>
        Task<IEnumerable<Booking>> GetUpcomingBookingsAsync();

        /// <summary>
        /// Finder aflyste bookinger
        /// </summary>
        Task<IEnumerable<Booking>> GetCancelledBookingsAsync();

        /// <summary>
        /// Finder gennemførte bookinger
        /// </summary>
        Task<IEnumerable<Booking>> GetCompletedBookingsAsync();

        /// <summary>
        /// Finder den seneste booking for et dyr
        /// </summary>
        Task<Booking> GetLatestBookingForAnimalAsync(int animalId);

        /// <summary>
        /// Finder den seneste booking for en kunde
        /// </summary>
        Task<Booking> GetLatestBookingForCustomerAsync(int customerId);
    }
} 
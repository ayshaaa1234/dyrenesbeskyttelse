using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Repository til håndtering af bookinger
    /// </summary>
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository() : base()
        {
        }

        /// <summary>
        /// Finder bookinger for et bestemt dyr
        /// </summary>
        public Task<IEnumerable<Booking>> GetByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            return Task.FromResult(_items.Where(b => b.AnimalId == animalId));
        }

        /// <summary>
        /// Finder bookinger for en bestemt kunde
        /// </summary>
        public Task<IEnumerable<Booking>> GetByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            return Task.FromResult(_items.Where(b => b.CustomerId == customerId));
        }

        /// <summary>
        /// Finder bookinger for en bestemt medarbejder
        /// </summary>
        public Task<IEnumerable<Booking>> GetByEmployeeIdAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            return Task.FromResult(_items.Where(b => b.EmployeeId == employeeId));
        }

        /// <summary>
        /// Finder bookinger baseret på datointerval
        /// </summary>
        public Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            if (startDate > DateTime.Now)
                throw new ArgumentException("Startdato kan ikke være i fremtiden");

            return Task.FromResult(_items.Where(b => 
                b.BookingDate >= startDate && b.BookingDate <= endDate));
        }

        /// <summary>
        /// Finder bookinger baseret på status
        /// </summary>
        public Task<IEnumerable<Booking>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status kan ikke være tom");

            if (!Enum.TryParse<Booking.BookingStatus>(status, true, out var bookingStatus))
                throw new ArgumentException("Ugyldig status");

            return Task.FromResult(_items.Where(b => b.Status == bookingStatus));
        }

        /// <summary>
        /// Finder bookinger baseret på type
        /// </summary>
        public Task<IEnumerable<Booking>> GetByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Type kan ikke være tom");

            return Task.FromResult(_items.Where(b => 
                b.Type.Contains(type, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder kommende bookinger
        /// </summary>
        public Task<IEnumerable<Booking>> GetUpcomingBookingsAsync()
        {
            return Task.FromResult(_items.Where(b => 
                b.BookingDate > DateTime.Now && 
                b.Status != Booking.BookingStatus.Cancelled));
        }

        /// <summary>
        /// Finder aflyste bookinger
        /// </summary>
        public Task<IEnumerable<Booking>> GetCancelledBookingsAsync()
        {
            return Task.FromResult(_items.Where(b => 
                b.Status == Booking.BookingStatus.Cancelled));
        }

        /// <summary>
        /// Finder gennemførte bookinger
        /// </summary>
        public Task<IEnumerable<Booking>> GetCompletedBookingsAsync()
        {
            return Task.FromResult(_items.Where(b => 
                b.Status == Booking.BookingStatus.Completed));
        }

        /// <summary>
        /// Finder den seneste booking for et dyr
        /// </summary>
        public Task<Booking> GetLatestBookingForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            var booking = _items
                .Where(b => b.AnimalId == animalId)
                .OrderByDescending(b => b.BookingDate)
                .FirstOrDefault();

            if (booking == null)
                throw new KeyNotFoundException($"Ingen bookinger fundet for dyr med ID: {animalId}");

            return Task.FromResult(booking);
        }

        /// <summary>
        /// Finder den seneste booking for en kunde
        /// </summary>
        public Task<Booking> GetLatestBookingForCustomerAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            var booking = _items
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .FirstOrDefault();

            if (booking == null)
                throw new KeyNotFoundException($"Ingen bookinger fundet for kunde med ID: {customerId}");

            return Task.FromResult(booking);
        }
    }
} 
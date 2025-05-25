using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Services
{
    /// <summary>
    /// Service til håndtering af bookinger
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        /// <summary>
        /// Konstruktør
        /// </summary>
        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        }

        /// <summary>
        /// Henter alle bookinger
        /// </summary>
        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        /// <summary>
        /// Henter en booking baseret på ID
        /// </summary>
        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                throw new KeyNotFoundException($"Ingen booking fundet med ID: {id}");

            return booking;
        }

        /// <summary>
        /// Opretter en ny booking
        /// </summary>
        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            ValidateBooking(booking);
            return await _bookingRepository.AddAsync(booking);
        }

        /// <summary>
        /// Opdaterer en eksisterende booking
        /// </summary>
        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            ValidateBooking(booking);
            return await _bookingRepository.UpdateAsync(booking);
        }

        /// <summary>
        /// Sletter en booking
        /// </summary>
        public async Task DeleteBookingAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            await _bookingRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Henter bookinger for et bestemt dyr
        /// </summary>
        public async Task<IEnumerable<Booking>> GetBookingsByAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            return await _bookingRepository.GetByAnimalIdAsync(animalId);
        }

        /// <summary>
        /// Henter bookinger for en bestemt kunde
        /// </summary>
        public async Task<IEnumerable<Booking>> GetBookingsByCustomerAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            return await _bookingRepository.GetByCustomerIdAsync(customerId);
        }

        /// <summary>
        /// Henter bookinger for en bestemt medarbejder
        /// </summary>
        public async Task<IEnumerable<Booking>> GetBookingsByEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            return await _bookingRepository.GetByEmployeeIdAsync(employeeId);
        }

        /// <summary>
        /// Henter bookinger i et datointerval
        /// </summary>
        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            return await _bookingRepository.GetByDateRangeAsync(startDate, endDate);
        }

        /// <summary>
        /// Henter bookinger baseret på status
        /// </summary>
        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status kan ikke være tom");

            return await _bookingRepository.GetByStatusAsync(status);
        }

        /// <summary>
        /// Henter bookinger baseret på type
        /// </summary>
        public async Task<IEnumerable<Booking>> GetBookingsByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Type kan ikke være tom");

            return await _bookingRepository.GetByTypeAsync(type);
        }

        /// <summary>
        /// Henter kommende bookinger
        /// </summary>
        public async Task<IEnumerable<Booking>> GetUpcomingBookingsAsync()
        {
            return await _bookingRepository.GetUpcomingBookingsAsync();
        }

        /// <summary>
        /// Henter aflyste bookinger
        /// </summary>
        public async Task<IEnumerable<Booking>> GetCancelledBookingsAsync()
        {
            return await _bookingRepository.GetCancelledBookingsAsync();
        }

        /// <summary>
        /// Henter gennemførte bookinger
        /// </summary>
        public async Task<IEnumerable<Booking>> GetCompletedBookingsAsync()
        {
            return await _bookingRepository.GetCompletedBookingsAsync();
        }

        /// <summary>
        /// Henter den seneste booking for et dyr
        /// </summary>
        public async Task<Booking> GetLatestBookingForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            return await _bookingRepository.GetLatestBookingForAnimalAsync(animalId);
        }

        /// <summary>
        /// Henter den seneste booking for en kunde
        /// </summary>
        public async Task<Booking> GetLatestBookingForCustomerAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            return await _bookingRepository.GetLatestBookingForCustomerAsync(customerId);
        }

        /// <summary>
        /// Bekræfter en booking
        /// </summary>
        public async Task ConfirmBookingAsync(int id)
        {
            var booking = await GetBookingByIdAsync(id);
            if (booking.Status == Booking.BookingStatus.Confirmed)
                throw new InvalidOperationException("Bookingen er allerede bekræftet");

            booking.Status = Booking.BookingStatus.Confirmed;
            await _bookingRepository.UpdateAsync(booking);
        }

        /// <summary>
        /// Aflyser en booking
        /// </summary>
        public async Task CancelBookingAsync(int id)
        {
            var booking = await GetBookingByIdAsync(id);
            if (booking.Status == Booking.BookingStatus.Cancelled)
                throw new InvalidOperationException("Bookingen er allerede aflyst");

            booking.Status = Booking.BookingStatus.Cancelled;
            await _bookingRepository.UpdateAsync(booking);
        }

        /// <summary>
        /// Marker en booking som gennemført
        /// </summary>
        public async Task CompleteBookingAsync(int id)
        {
            var booking = await GetBookingByIdAsync(id);
            if (booking.Status == Booking.BookingStatus.Completed)
                throw new InvalidOperationException("Bookingen er allerede gennemført");

            booking.Status = Booking.BookingStatus.Completed;
            await _bookingRepository.UpdateAsync(booking);
        }

        /// <summary>
        /// Tilføjer en booking til venteliste
        /// </summary>
        public async Task WaitlistBookingAsync(int id)
        {
            var booking = await GetBookingByIdAsync(id);
            if (booking.Status == Booking.BookingStatus.Waitlisted)
                throw new InvalidOperationException("Bookingen er allerede på venteliste");

            booking.Status = Booking.BookingStatus.Waitlisted;
            await _bookingRepository.UpdateAsync(booking);
        }

        /// <summary>
        /// Validerer en booking
        /// </summary>
        private void ValidateBooking(Booking booking)
        {
            if (booking.AnimalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            if (booking.CustomerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            if (booking.EmployeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            if (booking.BookingDate < DateTime.Now)
                throw new ArgumentException("Bookingdato kan ikke være i fortiden");

            if (booking.DurationMinutes <= 0)
                throw new ArgumentException("Varighed skal være større end 0");

            if (string.IsNullOrWhiteSpace(booking.Type))
                throw new ArgumentException("Type kan ikke være tom");

            if (string.IsNullOrWhiteSpace(booking.Purpose))
                throw new ArgumentException("Formål kan ikke være tomt");
        }
    }
} 
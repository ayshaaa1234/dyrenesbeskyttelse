using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Repository til håndtering af kunder
    /// </summary>
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public CustomerRepository() : base()
        {
        }

        /// <summary>
        /// Finder kunder baseret på navn
        /// </summary>
        public Task<IEnumerable<Customer>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt");

            return Task.FromResult(_items.Where(c => 
                c.Name.Contains(name, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder en kunde baseret på email
        /// </summary>
        public Task<Customer> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email kan ikke være tom");

            if (!EmailRegex.IsMatch(email))
                throw new ArgumentException("Ugyldigt email format");

            var customer = _items.FirstOrDefault(c => 
                c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (customer == null)
                throw new KeyNotFoundException($"Ingen kunde fundet med email: {email}");

            return Task.FromResult(customer);
        }

        /// <summary>
        /// Finder kunder baseret på telefonnummer
        /// </summary>
        public Task<IEnumerable<Customer>> GetByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefonnummer kan ikke være tomt");

            // Fjern alle ikke-numeriske tegn for at gøre søgningen mere fleksibel
            var normalizedPhone = new string(phone.Where(char.IsDigit).ToArray());
            
            return Task.FromResult(_items.Where(c => 
                new string(c.Phone.Where(char.IsDigit).ToArray()).Contains(normalizedPhone)));
        }

        /// <summary>
        /// Finder kunder baseret på adresse
        /// </summary>
        public Task<IEnumerable<Customer>> GetByAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Adresse kan ikke være tom");

            return Task.FromResult(_items.Where(c => 
                c.Address.Contains(address, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder kunder baseret på postnummer
        /// </summary>
        public Task<IEnumerable<Customer>> GetByPostalCodeAsync(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postnummer kan ikke være tomt");

            // Fjern alle ikke-numeriske tegn for at gøre søgningen mere fleksibel
            var normalizedPostalCode = new string(postalCode.Where(char.IsDigit).ToArray());
            
            return Task.FromResult(_items.Where(c => 
                new string(c.PostalCode.Where(char.IsDigit).ToArray()).Contains(normalizedPostalCode)));
        }

        /// <summary>
        /// Finder kunder baseret på by
        /// </summary>
        public Task<IEnumerable<Customer>> GetByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("By kan ikke være tom");

            return Task.FromResult(_items.Where(c => 
                c.City.Contains(city, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder kunder baseret på medlemskabsstatus
        /// </summary>
        public Task<IEnumerable<Customer>> GetByMembershipStatusAsync(bool isMember)
        {
            return Task.FromResult(_items.Where(c => c.IsMember == isMember));
        }

        /// <summary>
        /// Finder kunder baseret på medlemskabstype
        /// </summary>
        public Task<IEnumerable<Customer>> GetByMembershipTypeAsync(string membershipType)
        {
            if (string.IsNullOrWhiteSpace(membershipType))
                throw new ArgumentException("Medlemskabstype kan ikke være tom");

            return Task.FromResult(_items.Where(c => 
                c.MembershipType.Equals(membershipType, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder kunder der er registreret i et bestemt datointerval
        /// </summary>
        public Task<IEnumerable<Customer>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            if (startDate > DateTime.Now)
                throw new ArgumentException("Startdato kan ikke være i fremtiden");

            return Task.FromResult(_items.Where(c => 
                c.RegistrationDate >= startDate && c.RegistrationDate <= endDate));
        }

        /// <summary>
        /// Finder kunder der har adopteret dyr
        /// </summary>
        public Task<IEnumerable<Customer>> GetCustomersWithAdoptionsAsync()
        {
            return Task.FromResult(_items.Where(c => c.Adoptions.Any()));
        }

        /// <summary>
        /// Finder kunder baseret på navn (søger i både fornavn og efternavn)
        /// </summary>
        public Task<IEnumerable<Customer>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt");

            return Task.FromResult(_items.Where(c => 
                c.Name.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                c.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                c.LastName.Contains(name, StringComparison.OrdinalIgnoreCase)));
        }
    }
} 
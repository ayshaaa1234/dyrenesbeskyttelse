using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for repository til håndtering af kunder
    /// </summary>
    public interface ICustomerRepository : IRepository<Customer>
    {
        /// <summary>
        /// Finder kunder baseret på navn
        /// </summary>
        Task<IEnumerable<Customer>> GetByNameAsync(string name);

        /// <summary>
        /// Finder en kunde baseret på email
        /// </summary>
        Task<Customer> GetByEmailAsync(string email);

        /// <summary>
        /// Finder kunder baseret på telefonnummer
        /// </summary>
        Task<IEnumerable<Customer>> GetByPhoneAsync(string phone);

        /// <summary>
        /// Finder kunder baseret på adresse
        /// </summary>
        Task<IEnumerable<Customer>> GetByAddressAsync(string address);

        /// <summary>
        /// Finder kunder baseret på postnummer
        /// </summary>
        Task<IEnumerable<Customer>> GetByPostalCodeAsync(string postalCode);

        /// <summary>
        /// Finder kunder baseret på by
        /// </summary>
        Task<IEnumerable<Customer>> GetByCityAsync(string city);

        /// <summary>
        /// Finder kunder baseret på medlemskabsstatus
        /// </summary>
        Task<IEnumerable<Customer>> GetByMembershipStatusAsync(bool isMember);

        /// <summary>
        /// Finder kunder baseret på medlemskabstype
        /// </summary>
        Task<IEnumerable<Customer>> GetByMembershipTypeAsync(string membershipType);

        /// <summary>
        /// Finder kunder der er registreret i et bestemt datointerval
        /// </summary>
        Task<IEnumerable<Customer>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder kunder der har adopteret dyr
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersWithAdoptionsAsync();

        /// <summary>
        /// Finder kunder baseret på navn (søger i både fornavn og efternavn)
        /// </summary>
        Task<IEnumerable<Customer>> SearchByNameAsync(string name);
    }
} 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for service til håndtering af kunder
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Henter alle kunder
        /// </summary>
        Task<IEnumerable<Customer>> GetAllCustomersAsync();

        /// <summary>
        /// Henter en kunde baseret på ID
        /// </summary>
        Task<Customer> GetCustomerByIdAsync(int id);

        /// <summary>
        /// Opretter en ny kunde
        /// </summary>
        Task<Customer> CreateCustomerAsync(Customer customer);

        /// <summary>
        /// Opdaterer en eksisterende kunde
        /// </summary>
        Task<Customer> UpdateCustomerAsync(Customer customer);

        /// <summary>
        /// Sletter en kunde
        /// </summary>
        Task DeleteCustomerAsync(int id);

        /// <summary>
        /// Henter kunder baseret på navn
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersByNameAsync(string name);

        /// <summary>
        /// Henter en kunde baseret på email
        /// </summary>
        Task<Customer> GetCustomerByEmailAsync(string email);

        /// <summary>
        /// Henter kunder baseret på telefonnummer
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersByPhoneAsync(string phone);

        /// <summary>
        /// Henter kunder baseret på adresse
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersByAddressAsync(string address);

        /// <summary>
        /// Henter kunder baseret på postnummer
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersByPostalCodeAsync(string postalCode);

        /// <summary>
        /// Henter kunder baseret på by
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersByCityAsync(string city);

        /// <summary>
        /// Henter kunder baseret på medlemskabsstatus
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersByMembershipStatusAsync(bool isMember);

        /// <summary>
        /// Henter kunder baseret på medlemskabstype
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersByMembershipTypeAsync(string membershipType);

        /// <summary>
        /// Henter kunder der er registreret i et bestemt datointerval
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter kunder der har adopteret dyr
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersWithAdoptionsAsync();

        /// <summary>
        /// Søger efter kunder baseret på navn
        /// </summary>
        Task<IEnumerable<Customer>> SearchCustomersByNameAsync(string name);

        /// <summary>
        /// Tilføjer et medlemskab til en kunde
        /// </summary>
        Task AddMembershipAsync(int customerId, string membershipType);

        /// <summary>
        /// Fjerner et medlemskab fra en kunde
        /// </summary>
        Task RemoveMembershipAsync(int customerId);

        /// <summary>
        /// Opdaterer en kundes medlemskabstype
        /// </summary>
        Task UpdateMembershipTypeAsync(int customerId, string membershipType);
    }
} 
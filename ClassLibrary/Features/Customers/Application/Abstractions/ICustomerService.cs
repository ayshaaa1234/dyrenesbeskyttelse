using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Customers.Core.Models; // Opdateret for Customer
using ClassLibrary.SharedKernel.Application.Abstractions; // Opdateret for IBaseUserService

namespace ClassLibrary.Features.Customers.Application.Abstractions // Opdateret namespace
{
    /// <summary>
    /// Interface for service til håndtering af kunder, udvider IBaseUserService
    /// </summary>
    public interface ICustomerService : IBaseUserService<Customer>
    {
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
        /// Henter kunder der har aktive adoptioner
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersWithActiveAdoptionsAsync();

        // Bemærk: Medlemskabs-relaterede operationer (AddMembershipAsync, RemoveMembershipAsync, UpdateMembershipTypeAsync) 
        // er fjernet herfra. Disse bør håndteres af en dedikeret IMembershipService.
    }
} 
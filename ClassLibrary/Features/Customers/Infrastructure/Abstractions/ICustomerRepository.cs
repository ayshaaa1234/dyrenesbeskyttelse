using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Customers.Core.Models; // For Customer
using ClassLibrary.SharedKernel.Persistence.Abstractions; // For IRepository<T>

namespace ClassLibrary.Features.Customers.Infrastructure.Abstractions // Opdateret namespace
{
    /// <summary>
    /// Interface for repository til håndtering af kunder
    /// </summary>
    public interface ICustomerRepository : IRepository<Customer>
    {
        /// <summary>
        /// Finder kunder baseret på navn (kan matche fornavn eller efternavn delvist)
        /// </summary>
        Task<IEnumerable<Customer>> GetByNameAsync(string name);

        /// <summary>
        /// Finder en kunde baseret på email (forventer eksakt match, ignorerer case)
        /// </summary>
        Task<Customer?> GetByEmailAsync(string email);

        /// <summary>
        /// Finder kunder baseret på telefonnummer (kan matche delvist)
        /// </summary>
        Task<IEnumerable<Customer>> GetByPhoneAsync(string phone);

        /// <summary>
        /// Finder kunder baseret på adresse (kan matche delvist)
        /// </summary>
        Task<IEnumerable<Customer>> GetByAddressAsync(string address);

        /// <summary>
        /// Finder kunder baseret på postnummer (forventer eksakt match)
        /// </summary>
        Task<IEnumerable<Customer>> GetByPostalCodeAsync(string postalCode);

        /// <summary>
        /// Finder kunder baseret på by (kan matche delvist)
        /// </summary>
        Task<IEnumerable<Customer>> GetByCityAsync(string city);

        /// <summary>
        /// Finder kunder der er registreret i et bestemt datointerval
        /// </summary>
        Task<IEnumerable<Customer>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder kunder der har mindst én aktiv adoption
        /// Denne metode vil kræve adgang til Adoption data, muligvis via en join eller separat query.
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomersWithActiveAdoptionsAsync();

        // SearchByNameAsync er meget lig GetByNameAsync. Overvej at konsolidere eller tydeliggøre forskel.
        // For nu, lad os antage GetByNameAsync er den primære søgning og omdøbe SearchByNameAsync til noget mere specifikt hvis nødvendigt,
        // eller fjerne den hvis den er redundant.
        // Jeg fjerner SearchByNameAsync for nu, da GetByNameAsync kan dække bred søgning.
    }
} 
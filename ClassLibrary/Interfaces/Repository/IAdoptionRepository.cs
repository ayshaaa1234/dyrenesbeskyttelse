using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for repository til håndtering af adoptioner
    /// </summary>
    public interface IAdoptionRepository : IRepository<Adoption>
    {
        /// <summary>
        /// Finder alle adoptioner for en bestemt kunde
        /// </summary>
        Task<IEnumerable<Adoption>> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// Finder alle adoptioner for et bestemt dyr
        /// </summary>
        Task<IEnumerable<Adoption>> GetByAnimalIdAsync(int animalId);

        /// <summary>
        /// Finder adoptioner i et bestemt datointerval
        /// </summary>
        Task<IEnumerable<Adoption>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder adoptioner baseret på status
        /// </summary>
        Task<IEnumerable<Adoption>> GetByStatusAsync(AdoptionStatus status);

        /// <summary>
        /// Finder adoptioner baseret på dyreart
        /// </summary>
        Task<IEnumerable<Adoption>> GetBySpeciesAsync(Species species);

        /// <summary>
        /// Finder adoptioner baseret på alder i år
        /// </summary>
        Task<IEnumerable<Adoption>> GetByAgeInYearsAsync(int years);

        /// <summary>
        /// Finder adoptioner baseret på alder i måneder
        /// </summary>
        Task<IEnumerable<Adoption>> GetByAgeInMonthsAsync(int months);

        /// <summary>
        /// Finder adoptioner baseret på alder i uger
        /// </summary>
        Task<IEnumerable<Adoption>> GetByAgeInWeeksAsync(int weeks);

        /// <summary>
        /// Finder adoptioner baseret på aldersinterval i år
        /// </summary>
        Task<IEnumerable<Adoption>> GetByAgeRangeInYearsAsync(int minYears, int maxYears);

        /// <summary>
        /// Finder adoptioner baseret på aldersinterval i måneder
        /// </summary>
        Task<IEnumerable<Adoption>> GetByAgeRangeInMonthsAsync(int minMonths, int maxMonths);

        /// <summary>
        /// Finder adoptioner baseret på aldersinterval i uger
        /// </summary>
        Task<IEnumerable<Adoption>> GetByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks);

        /// <summary>
        /// Finder adoptioner baseret på adoptionstype
        /// </summary>
        Task<IEnumerable<Adoption>> GetByAdoptionTypeAsync(string adoptionType);

        /// <summary>
        /// Finder adoptioner baseret på medarbejder
        /// </summary>
        Task<IEnumerable<Adoption>> GetByEmployeeIdAsync(int employeeId);

        /// <summary>
        /// Finder alle adoptioner for en bestemt adopter
        /// </summary>
        Task<IEnumerable<Adoption>> GetByAdopterEmailAsync(string email);

        /// <summary>
        /// Finder den seneste adoption for et dyr
        /// </summary>
        /// <param name="animalId">ID på dyret</param>
        /// <returns>Den seneste adoption for dyret</returns>
        /// <exception cref="KeyNotFoundException">Kastes når ingen adoption findes for dyret</exception>
        Task<Adoption> GetLatestAdoptionForAnimalAsync(int animalId);
    }
} 
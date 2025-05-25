using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for repository til håndtering af dyr
    /// </summary>
    public interface IAnimalRepository : IRepository<Animal>
    {
        /// <summary>
        /// Finder dyr baseret på dyreart
        /// </summary>
        Task<IEnumerable<Animal>> GetBySpeciesAsync(Species species);

        /// <summary>
        /// Finder dyr baseret på alder i år
        /// </summary>
        Task<IEnumerable<Animal>> GetByAgeInYearsAsync(int years);

        /// <summary>
        /// Finder dyr baseret på alder i måneder
        /// </summary>
        Task<IEnumerable<Animal>> GetByAgeInMonthsAsync(int months);

        /// <summary>
        /// Finder dyr baseret på alder i uger
        /// </summary>
        Task<IEnumerable<Animal>> GetByAgeInWeeksAsync(int weeks);

        /// <summary>
        /// Finder dyr baseret på køn
        /// </summary>
        Task<IEnumerable<Animal>> GetByGenderAsync(string gender);

        /// <summary>
        /// Finder dyr baseret på vægtinterval
        /// </summary>
        Task<IEnumerable<Animal>> GetByWeightRangeAsync(decimal minWeight, decimal maxWeight);

        /// <summary>
        /// Finder dyr baseret på aldersinterval i år
        /// </summary>
        Task<IEnumerable<Animal>> GetByAgeRangeInYearsAsync(int minYears, int maxYears);

        /// <summary>
        /// Finder dyr baseret på aldersinterval i måneder
        /// </summary>
        Task<IEnumerable<Animal>> GetByAgeRangeInMonthsAsync(int minMonths, int maxMonths);

        /// <summary>
        /// Finder dyr baseret på aldersinterval i uger
        /// </summary>
        Task<IEnumerable<Animal>> GetByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks);

        /// <summary>
        /// Finder dyr baseret på adoptionsstatus
        /// </summary>
        Task<IEnumerable<Animal>> GetByAdoptionStatusAsync(bool isAdopted);

        /// <summary>
        /// Finder dyr baseret på indkomstdato
        /// </summary>
        Task<IEnumerable<Animal>> GetByIntakeDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder dyr baseret på navn
        /// </summary>
        Task<IEnumerable<Animal>> GetByNameAsync(string name);

        /// <summary>
        /// Finder dyr baseret på race
        /// </summary>
        Task<IEnumerable<Animal>> GetByBreedAsync(string breed);

        /// <summary>
        /// Finder alle dyr der er adopteret
        /// </summary>
        Task<IEnumerable<Animal>> GetAdoptedAnimalsAsync();

        /// <summary>
        /// Finder alle dyr der ikke er adopteret
        /// </summary>
        Task<IEnumerable<Animal>> GetAvailableAnimalsAsync();

        /// <summary>
        /// Finder dyr der skal til lægen i en bestemt uge
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsWithVetAppointmentsInWeekAsync(int weekNumber);
    }
} 
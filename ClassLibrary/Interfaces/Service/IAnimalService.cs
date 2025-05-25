using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for service til håndtering af dyr
    /// </summary>
    public interface IAnimalService
    {
        /// <summary>
        /// Henter alle dyr
        /// </summary>
        Task<IEnumerable<Animal>> GetAllAnimalsAsync();

        /// <summary>
        /// Henter et dyr baseret på ID
        /// </summary>
        Task<Animal> GetAnimalByIdAsync(int id);

        /// <summary>
        /// Opretter et nyt dyr
        /// </summary>
        Task<Animal> CreateAnimalAsync(Animal animal);

        /// <summary>
        /// Opdaterer et eksisterende dyr
        /// </summary>
        Task<Animal> UpdateAnimalAsync(Animal animal);

        /// <summary>
        /// Sletter et dyr
        /// </summary>
        Task DeleteAnimalAsync(int id);

        /// <summary>
        /// Henter dyr baseret på dyreart
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsBySpeciesAsync(Species species);

        /// <summary>
        /// Henter dyr baseret på alder i år
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeInYearsAsync(int years);

        /// <summary>
        /// Henter dyr baseret på alder i måneder
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeInMonthsAsync(int months);

        /// <summary>
        /// Henter dyr baseret på alder i uger
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeInWeeksAsync(int weeks);

        /// <summary>
        /// Henter dyr baseret på køn
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByGenderAsync(string gender);

        /// <summary>
        /// Henter dyr baseret på vægtinterval
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByWeightRangeAsync(decimal minWeight, decimal maxWeight);

        /// <summary>
        /// Henter dyr baseret på aldersinterval i år
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInYearsAsync(int minYears, int maxYears);

        /// <summary>
        /// Henter dyr baseret på aldersinterval i måneder
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInMonthsAsync(int minMonths, int maxMonths);

        /// <summary>
        /// Henter dyr baseret på aldersinterval i uger
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks);

        /// <summary>
        /// Henter dyr baseret på adoptionsstatus
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByAdoptionStatusAsync(bool isAdopted);

        /// <summary>
        /// Henter dyr baseret på indkomstdato
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByIntakeDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter dyr baseret på navn
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByNameAsync(string name);

        /// <summary>
        /// Henter dyr baseret på race
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsByBreedAsync(string breed);

        /// <summary>
        /// Henter alle adopterede dyr
        /// </summary>
        Task<IEnumerable<Animal>> GetAdoptedAnimalsAsync();

        /// <summary>
        /// Henter alle tilgængelige dyr
        /// </summary>
        Task<IEnumerable<Animal>> GetAvailableAnimalsAsync();

        /// <summary>
        /// Henter dyr der skal til lægen i en bestemt uge
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsWithVetAppointmentsInWeekAsync(int weekNumber);

        /// <summary>
        /// Markerer et dyr som adopteret
        /// </summary>
        Task MarkAnimalAsAdoptedAsync(int animalId, int customerId);

        /// <summary>
        /// Markerer et dyr som ikke adopteret
        /// </summary>
        Task MarkAnimalAsAvailableAsync(int animalId);

        /// <summary>
        /// Tilføjer en sundhedsjournal til et dyr
        /// </summary>
        Task AddHealthRecordAsync(int animalId, HealthRecord healthRecord);

        /// <summary>
        /// Tilføjer et besøg til et dyr
        /// </summary>
        Task AddVisitLogAsync(int animalId, VisitLog visitLog);
    }
} 
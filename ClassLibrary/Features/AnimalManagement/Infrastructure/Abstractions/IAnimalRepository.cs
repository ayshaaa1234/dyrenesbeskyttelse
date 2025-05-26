using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using ClassLibrary.SharedKernel.Persistence.Abstractions;

namespace ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions
{
    /// <summary>
    /// Interface for repository til håndtering af dyredata.
    /// </summary>
    public interface IAnimalRepository : IRepository<Animal>
    {
        /// <summary>
        /// Henter alle dyr, der er tilgængelige for adoption.
        /// </summary>
        Task<IEnumerable<Animal>> GetAvailableAnimalsAsync();
        /// <summary>
        /// Henter alle dyr, der er blevet adopteret.
        /// </summary>
        Task<IEnumerable<Animal>> GetAdoptedAnimalsAsync();
        /// <summary>
        /// Henter dyr baseret på deres art.
        /// </summary>
        /// <param name="species">Arten der søges efter.</param>
        Task<IEnumerable<Animal>> GetAnimalsBySpeciesAsync(Species species);
        /// <summary>
        /// Henter dyr baseret på deres alder i hele år.
        /// </summary>
        /// <param name="age">Alderen i år.</param>
        Task<IEnumerable<Animal>> GetAnimalsByAgeInYearsAsync(int age);
        /// <summary>
        /// Henter dyr baseret på deres alder i hele måneder.
        /// </summary>
        /// <param name="age">Alderen i måneder.</param>
        Task<IEnumerable<Animal>> GetAnimalsByAgeInMonthsAsync(int age);
        /// <summary>
        /// Henter dyr baseret på deres alder i hele uger.
        /// </summary>
        /// <param name="ageInWeeks">Alderen i uger.</param>
        Task<IEnumerable<Animal>> GetAnimalsByAgeInWeeksAsync(int ageInWeeks);
        /// <summary>
        /// Henter dyr baseret på deres køn.
        /// </summary>
        /// <param name="gender">Kønnet der søges efter.</param>
        Task<IEnumerable<Animal>> GetAnimalsByGenderAsync(Gender gender);
        /// <summary>
        /// Henter dyr inden for et specificeret vægtinterval.
        /// </summary>
        /// <param name="minWeight">Minimumsvægt.</param>
        /// <param name="maxWeight">Maksimumsvægt.</param>
        Task<IEnumerable<Animal>> GetAnimalsByWeightRangeAsync(decimal minWeight, decimal maxWeight);
        /// <summary>
        /// Henter dyr inden for et specificeret aldersinterval i år.
        /// </summary>
        /// <param name="minAge">Minimumsalder i år.</param>
        /// <param name="maxAge">Maksimumsalder i år.</param>
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInYearsAsync(int minAge, int maxAge);
        /// <summary>
        /// Henter dyr inden for et specificeret aldersinterval i måneder.
        /// </summary>
        /// <param name="minAge">Minimumsalder i måneder.</param>
        /// <param name="maxAge">Maksimumsalder i måneder.</param>
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInMonthsAsync(int minAge, int maxAge);
        /// <summary>
        /// Henter dyr inden for et specificeret aldersinterval i uger.
        /// </summary>
        /// <param name="minAge">Minimumsalder i uger.</param>
        /// <param name="maxAge">Maksimumsalder i uger.</param>
        Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInWeeksAsync(int minAge, int maxAge);
        /// <summary>
        /// Henter dyr baseret på deres adoptionsstatus.
        /// </summary>
        /// <param name="status">Adoptionsstatus der søges efter.</param>
        Task<IEnumerable<Animal>> GetAnimalsByAdoptionStatusAsync(AnimalStatus status); // Bruger AnimalStatus fra Core.Enums
        /// <summary>
        /// Henter dyr baseret på deres indtagelsesdato inden for et interval.
        /// </summary>
        /// <param name="startDate">Startdato for intervallet.</param>
        /// <param name="endDate">Slutdato for intervallet.</param>
        Task<IEnumerable<Animal>> GetAnimalsByIntakeDateRangeAsync(DateTime startDate, DateTime endDate);
        /// <summary>
        /// Henter dyr baseret på deres navn (delvis matchning understøttes typisk).
        /// </summary>
        /// <param name="name">Navnet eller en del af navnet der søges efter.</param>
        Task<IEnumerable<Animal>> GetAnimalsByNameAsync(string name);
        /// <summary>
        /// Henter dyr baseret på deres race (delvis matchning understøttes typisk).
        /// </summary>
        /// <param name="breed">Racen eller en del af racen der søges efter.</param>
        Task<IEnumerable<Animal>> GetAnimalsByBreedAsync(string breed);
        /// <summary>
        /// Henter dyr, der har behov for vaccination.
        /// </summary>
        Task<IEnumerable<Animal>> GetAnimalsNeedingVaccinationAsync(); // Flyttet fra Health Record Operations
        /// <summary>
        /// Henter en liste af dyr baseret på en samling af ID'er.
        /// </summary>
        /// <param name="ids">En samling af dyre-ID'er.</param>
        Task<IEnumerable<Animal>> GetAnimalsByIdsAsync(IEnumerable<int> ids);
    }
} 
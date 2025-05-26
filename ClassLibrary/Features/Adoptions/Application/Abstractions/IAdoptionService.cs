using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Features.Adoptions.Core.Models;
using ClassLibrary.Features.Adoptions.Core.Enums;
using ClassLibrary.Features.AnimalManagement.Core.Enums; // For Species
// using ClassLibrary.Features.AnimalManagement.Core.Models; // For Species, tilføjes senere

namespace ClassLibrary.Features.Adoptions.Application.Abstractions
{
    /// <summary>
    /// Interface for service til håndtering af adoptioner
    /// </summary>
    public interface IAdoptionService
    {
        /// <summary>
        /// Henter alle adoptioner
        /// </summary>
        Task<IEnumerable<Adoption>> GetAllAdoptionsAsync();

        /// <summary>
        /// Henter en adoption baseret på ID
        /// </summary>
        Task<Adoption> GetAdoptionByIdAsync(int id);

        /// <summary>
        /// Opretter en ny adoption
        /// </summary>
        Task<Adoption> CreateAdoptionAsync(Adoption adoption);

        /// <summary>
        /// Opdaterer en eksisterende adoption
        /// </summary>
        Task<Adoption> UpdateAdoptionAsync(Adoption adoption);

        /// <summary>
        /// Sletter en adoption
        /// </summary>
        Task DeleteAdoptionAsync(int id);

        /// <summary>
        /// Henter adoptioner for en bestemt kunde
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByCustomerAsync(int customerId);

        /// <summary>
        /// Henter adoptioner for et bestemt dyr
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByAnimalAsync(int animalId);

        /// <summary>
        /// Henter adoptioner i et datointerval
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter adoptioner baseret på status
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByStatusAsync(AdoptionStatus status);

        /// <summary>
        /// Henter adoptioner baseret på dyreart
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsBySpeciesAsync(AnimalManagement.Core.Enums.Species species); // Opdateret type

        /// <summary>
        /// Henter adoptioner baseret på alder i år
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByAgeInYearsAsync(int years);

        /// <summary>
        /// Henter adoptioner baseret på alder i måneder
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByAgeInMonthsAsync(int months);

        /// <summary>
        /// Henter adoptioner baseret på alder i uger
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByAgeInWeeksAsync(int weeks);

        /// <summary>
        /// Henter adoptioner baseret på aldersinterval i år
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInYearsAsync(int minYears, int maxYears);

        /// <summary>
        /// Henter adoptioner baseret på aldersinterval i måneder
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInMonthsAsync(int minMonths, int maxMonths);

        /// <summary>
        /// Henter adoptioner baseret på aldersinterval i uger
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks);

        /// <summary>
        /// Henter adoptioner baseret på adoptionstype
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByTypeAsync(string adoptionType);

        /// <summary>
        /// Henter adoptioner baseret på medarbejder
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByEmployeeAsync(int employeeId);

        /// <summary>
        /// Henter adoptioner baseret på adopters email
        /// </summary>
        Task<IEnumerable<Adoption>> GetAdoptionsByAdopterEmailAsync(string email);

        /// <summary>
        /// Henter den seneste adoption for et dyr
        /// </summary>
        Task<Adoption> GetLatestAdoptionForAnimalAsync(int animalId);

        /// <summary>
        /// Godkender en adoption
        /// </summary>
        Task ApproveAdoptionAsync(int adoptionId, int employeeId);

        /// <summary>
        /// Afviser en adoption
        /// </summary>
        Task RejectAdoptionAsync(int adoptionId, int employeeId);

        /// <summary>
        /// Gennemfører en adoption
        /// </summary>
        Task CompleteAdoptionAsync(int adoptionId);

        /// <summary>
        /// Annullerer en adoption
        /// </summary>
        /// <param name="adoptionId">ID på adoptionen der skal annulleres.</param>
        /// <param name="employeeId">ID på medarbejderen der annullerer.</param>
        /// <param name="reason">Årsag til annullering.</param>
        Task CancelAdoptionAsync(int adoptionId, int employeeId, string reason);
    }
} 
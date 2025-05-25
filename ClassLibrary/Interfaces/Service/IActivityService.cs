using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Services
{
    /// <summary>
    /// Interface for service til håndtering af aktiviteter
    /// </summary>
    public interface IActivityService
    {
        /// <summary>
        /// Henter alle aktiviteter
        /// </summary>
        Task<IEnumerable<Activity>> GetAllActivitiesAsync();

        /// <summary>
        /// Henter en aktivitet baseret på ID
        /// </summary>
        Task<Activity> GetActivityByIdAsync(int id);

        /// <summary>
        /// Opretter en ny aktivitet
        /// </summary>
        Task<Activity> CreateActivityAsync(Activity activity);

        /// <summary>
        /// Opdaterer en eksisterende aktivitet
        /// </summary>
        Task<Activity> UpdateActivityAsync(Activity activity);

        /// <summary>
        /// Sletter en aktivitet
        /// </summary>
        Task DeleteActivityAsync(int id);

        /// <summary>
        /// Henter alle kommende aktiviteter
        /// </summary>
        Task<IEnumerable<Activity>> GetUpcomingActivitiesAsync();

        /// <summary>
        /// Henter aktiviteter for en bestemt medarbejder
        /// </summary>
        Task<IEnumerable<Activity>> GetActivitiesByEmployeeAsync(int employeeId);

        /// <summary>
        /// Henter aktiviteter i et datointerval
        /// </summary>
        Task<IEnumerable<Activity>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Henter aktiviteter baseret på status
        /// </summary>
        Task<IEnumerable<Activity>> GetActivitiesByStatusAsync(ActivityStatus status);

        /// <summary>
        /// Henter aktiviteter med ledige pladser
        /// </summary>
        Task<IEnumerable<Activity>> GetActivitiesWithAvailableSpotsAsync();

        /// <summary>
        /// Henter aktiviteter baseret på lokation
        /// </summary>
        Task<IEnumerable<Activity>> GetActivitiesByLocationAsync(string location);

        /// <summary>
        /// Henter aktiviteter baseret på prisinterval
        /// </summary>
        Task<IEnumerable<Activity>> GetActivitiesByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        /// <summary>
        /// Henter aktiviteter for en bestemt deltager
        /// </summary>
        Task<IEnumerable<Activity>> GetActivitiesForParticipantAsync(int participantId);

        /// <summary>
        /// Tilføjer en deltager til en aktivitet
        /// </summary>
        Task AddParticipantToActivityAsync(int activityId, int participantId);

        /// <summary>
        /// Fjerner en deltager fra en aktivitet
        /// </summary>
        Task RemoveParticipantFromActivityAsync(int activityId, int participantId);

        /// <summary>
        /// Tjekker om en deltager er tilmeldt en aktivitet
        /// </summary>
        Task<bool> IsParticipantRegisteredAsync(int activityId, int participantId);
    }
} 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace ClassLibrary.Interfaces
{
    /// <summary>
    /// Interface for repository til håndtering af aktiviteter
    /// </summary>
    public interface IActivityRepository : IRepository<Activity>
    {
        /// <summary>
        /// Finder alle kommende aktiviteter
        /// </summary>
        Task<IEnumerable<Activity>> GetUpcomingActivitiesAsync();

        /// <summary>
        /// Finder aktiviteter for en bestemt medarbejder
        /// </summary>
        Task<IEnumerable<Activity>> GetByEmployeeIdAsync(int employeeId);

        /// <summary>
        /// Finder aktiviteter i et bestemt datointerval
        /// </summary>
        Task<IEnumerable<Activity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Finder aktiviteter baseret på status
        /// </summary>
        Task<IEnumerable<Activity>> GetByStatusAsync(ActivityStatus status);

        /// <summary>
        /// Finder aktiviteter der har ledige pladser
        /// </summary>
        Task<IEnumerable<Activity>> GetActivitiesWithAvailableSpotsAsync();

        /// <summary>
        /// Finder aktiviteter baseret på lokation
        /// </summary>
        Task<IEnumerable<Activity>> GetByLocationAsync(string location);

        /// <summary>
        /// Finder aktiviteter baseret på prisinterval
        /// </summary>
        Task<IEnumerable<Activity>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        /// <summary>
        /// Finder aktiviteter hvor en bestemt kunde er tilmeldt
        /// </summary>
        Task<IEnumerable<Activity>> GetActivitiesForParticipantAsync(int participantId);

        /// <summary>
        /// Tilføjer en deltager til en aktivitet
        /// </summary>
        Task AddParticipantAsync(int activityId, int participantId);

        /// <summary>
        /// Fjerner en deltager fra en aktivitet
        /// </summary>
        Task RemoveParticipantAsync(int activityId, int participantId);

        /// <summary>
        /// Tjekker om en deltager er tilmeldt en aktivitet
        /// </summary>
        Task<bool> IsParticipantRegisteredAsync(int activityId, int participantId);
    }
} 
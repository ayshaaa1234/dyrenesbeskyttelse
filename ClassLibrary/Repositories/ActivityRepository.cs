using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Repository til håndtering af aktiviteter
    /// </summary>
    public class ActivityRepository : Repository<Activity>, IActivityRepository
    {
        public ActivityRepository() : base()
        {
        }

        /// <summary>
        /// Finder alle kommende aktiviteter
        /// </summary>
        public Task<IEnumerable<Activity>> GetUpcomingActivitiesAsync()
        {
            var now = DateTime.Now;
            return Task.FromResult(_items.Where(a => 
                a.ActivityDate > now && 
                a.Status == ActivityStatus.Planned));
        }

        /// <summary>
        /// Finder aktiviteter for en bestemt medarbejder
        /// </summary>
        public Task<IEnumerable<Activity>> GetByEmployeeIdAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            return Task.FromResult(_items.Where(a => a.EmployeeId == employeeId));
        }

        /// <summary>
        /// Finder aktiviteter i et bestemt datointerval
        /// </summary>
        public Task<IEnumerable<Activity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            return Task.FromResult(_items.Where(a => 
                a.ActivityDate >= startDate && a.ActivityDate <= endDate));
        }

        /// <summary>
        /// Finder aktiviteter baseret på status
        /// </summary>
        public Task<IEnumerable<Activity>> GetByStatusAsync(ActivityStatus status)
        {
            return Task.FromResult(_items.Where(a => a.Status == status));
        }

        /// <summary>
        /// Finder aktiviteter der har ledige pladser
        /// </summary>
        public Task<IEnumerable<Activity>> GetActivitiesWithAvailableSpotsAsync()
        {
            return Task.FromResult(_items.Where(a => 
                !a.IsFullyBooked && 
                a.Status == ActivityStatus.Planned));
        }

        /// <summary>
        /// Finder aktiviteter baseret på lokation
        /// </summary>
        public Task<IEnumerable<Activity>> GetByLocationAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Lokation kan ikke være tom");

            return Task.FromResult(_items.Where(a => 
                a.Location.Equals(location, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder aktiviteter baseret på prisinterval
        /// </summary>
        public Task<IEnumerable<Activity>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0)
                throw new ArgumentException("Minimumspris kan ikke være negativ");
            if (maxPrice < minPrice)
                throw new ArgumentException("Maksimumspris skal være større end minimumspris");

            return Task.FromResult(_items.Where(a => 
                a.Price >= minPrice && a.Price <= maxPrice));
        }

        /// <summary>
        /// Finder aktiviteter hvor en bestemt kunde er tilmeldt
        /// </summary>
        public Task<IEnumerable<Activity>> GetActivitiesForParticipantAsync(int participantId)
        {
            if (participantId <= 0)
                throw new ArgumentException("ParticipantId skal være større end 0");

            return Task.FromResult(_items.Where(a => 
                a.ParticipantIds.Contains(participantId)));
        }

        /// <summary>
        /// Tilføjer en deltager til en aktivitet
        /// </summary>
        public async Task AddParticipantAsync(int activityId, int participantId)
        {
            if (activityId <= 0)
                throw new ArgumentException("ActivityId skal være større end 0");
            if (participantId <= 0)
                throw new ArgumentException("ParticipantId skal være større end 0");

            var activity = await GetByIdAsync(activityId);
            if (activity == null)
                throw new KeyNotFoundException($"Ingen aktivitet fundet med ID: {activityId}");

            activity.AddParticipant(participantId);
        }

        /// <summary>
        /// Fjerner en deltager fra en aktivitet
        /// </summary>
        public async Task RemoveParticipantAsync(int activityId, int participantId)
        {
            if (activityId <= 0)
                throw new ArgumentException("ActivityId skal være større end 0");
            if (participantId <= 0)
                throw new ArgumentException("ParticipantId skal være større end 0");

            var activity = await GetByIdAsync(activityId);
            if (activity == null)
                throw new KeyNotFoundException($"Ingen aktivitet fundet med ID: {activityId}");

            activity.RemoveParticipant(participantId);
        }

        /// <summary>
        /// Tjekker om en deltager er tilmeldt en aktivitet
        /// </summary>
        public async Task<bool> IsParticipantRegisteredAsync(int activityId, int participantId)
        {
            if (activityId <= 0)
                throw new ArgumentException("ActivityId skal være større end 0");
            if (participantId <= 0)
                throw new ArgumentException("ParticipantId skal være større end 0");

            var activity = await GetByIdAsync(activityId);
            if (activity == null)
                throw new KeyNotFoundException($"Ingen aktivitet fundet med ID: {activityId}");

            return activity.IsParticipantRegistered(participantId);
        }
    }
} 
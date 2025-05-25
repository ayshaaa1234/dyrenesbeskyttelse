using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Services
{
    /// <summary>
    /// Service til håndtering af aktiviteter
    /// </summary>
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;

        /// <summary>
        /// Konstruktør
        /// </summary>
        public ActivityService(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        /// <summary>
        /// Henter alle aktiviteter
        /// </summary>
        public async Task<IEnumerable<Activity>> GetAllActivitiesAsync()
        {
            return await _activityRepository.GetAllAsync();
        }

        /// <summary>
        /// Henter en aktivitet baseret på ID
        /// </summary>
        public async Task<Activity> GetActivityByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            var activity = await _activityRepository.GetByIdAsync(id);
            if (activity == null)
                throw new KeyNotFoundException($"Ingen aktivitet fundet med ID: {id}");

            return activity;
        }

        /// <summary>
        /// Opretter en ny aktivitet
        /// </summary>
        public async Task<Activity> CreateActivityAsync(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            ValidateActivity(activity);
            return await _activityRepository.AddAsync(activity);
        }

        /// <summary>
        /// Opdaterer en eksisterende aktivitet
        /// </summary>
        public async Task<Activity> UpdateActivityAsync(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            ValidateActivity(activity);
            return await _activityRepository.UpdateAsync(activity);
        }

        /// <summary>
        /// Sletter en aktivitet
        /// </summary>
        public async Task DeleteActivityAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0");

            await _activityRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Henter alle kommende aktiviteter
        /// </summary>
        public async Task<IEnumerable<Activity>> GetUpcomingActivitiesAsync()
        {
            return await _activityRepository.GetUpcomingActivitiesAsync();
        }

        /// <summary>
        /// Henter aktiviteter for en bestemt medarbejder
        /// </summary>
        public async Task<IEnumerable<Activity>> GetActivitiesByEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            return await _activityRepository.GetByEmployeeIdAsync(employeeId);
        }

        /// <summary>
        /// Henter aktiviteter i et datointerval
        /// </summary>
        public async Task<IEnumerable<Activity>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            return await _activityRepository.GetByDateRangeAsync(startDate, endDate);
        }

        /// <summary>
        /// Henter aktiviteter baseret på status
        /// </summary>
        public async Task<IEnumerable<Activity>> GetActivitiesByStatusAsync(ActivityStatus status)
        {
            return await _activityRepository.GetByStatusAsync(status);
        }

        /// <summary>
        /// Henter aktiviteter med ledige pladser
        /// </summary>
        public async Task<IEnumerable<Activity>> GetActivitiesWithAvailableSpotsAsync()
        {
            return await _activityRepository.GetActivitiesWithAvailableSpotsAsync();
        }

        /// <summary>
        /// Henter aktiviteter baseret på lokation
        /// </summary>
        public async Task<IEnumerable<Activity>> GetActivitiesByLocationAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Lokation kan ikke være tom");

            return await _activityRepository.GetByLocationAsync(location);
        }

        /// <summary>
        /// Henter aktiviteter baseret på prisinterval
        /// </summary>
        public async Task<IEnumerable<Activity>> GetActivitiesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0)
                throw new ArgumentException("Minimumspris kan ikke være negativ");
            if (maxPrice < minPrice)
                throw new ArgumentException("Maksimumspris skal være større end minimumspris");

            return await _activityRepository.GetByPriceRangeAsync(minPrice, maxPrice);
        }

        /// <summary>
        /// Henter aktiviteter for en bestemt deltager
        /// </summary>
        public async Task<IEnumerable<Activity>> GetActivitiesForParticipantAsync(int participantId)
        {
            if (participantId <= 0)
                throw new ArgumentException("ParticipantId skal være større end 0");

            return await _activityRepository.GetActivitiesForParticipantAsync(participantId);
        }

        /// <summary>
        /// Tilføjer en deltager til en aktivitet
        /// </summary>
        public async Task AddParticipantToActivityAsync(int activityId, int participantId)
        {
            if (activityId <= 0)
                throw new ArgumentException("ActivityId skal være større end 0");
            if (participantId <= 0)
                throw new ArgumentException("ParticipantId skal være større end 0");

            await _activityRepository.AddParticipantAsync(activityId, participantId);
        }

        /// <summary>
        /// Fjerner en deltager fra en aktivitet
        /// </summary>
        public async Task RemoveParticipantFromActivityAsync(int activityId, int participantId)
        {
            if (activityId <= 0)
                throw new ArgumentException("ActivityId skal være større end 0");
            if (participantId <= 0)
                throw new ArgumentException("ParticipantId skal være større end 0");

            await _activityRepository.RemoveParticipantAsync(activityId, participantId);
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

            return await _activityRepository.IsParticipantRegisteredAsync(activityId, participantId);
        }

        /// <summary>
        /// Validerer en aktivitet
        /// </summary>
        private void ValidateActivity(Activity activity)
        {
            if (string.IsNullOrWhiteSpace(activity.Name))
                throw new ArgumentException("Aktivitetsnavn kan ikke være tomt");

            if (string.IsNullOrWhiteSpace(activity.Description))
                throw new ArgumentException("Beskrivelse kan ikke være tom");

            if (activity.ActivityDate < DateTime.Now)
                throw new ArgumentException("Aktivitetsdato kan ikke være i fortiden");

            if (activity.DurationMinutes <= 0)
                throw new ArgumentException("Varighed skal være større end 0");

            if (activity.MaxParticipants <= 0)
                throw new ArgumentException("Maksimalt antal deltagere skal være større end 0");

            if (activity.EmployeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            if (string.IsNullOrWhiteSpace(activity.Location))
                throw new ArgumentException("Lokation kan ikke være tom");

            if (activity.Price < 0)
                throw new ArgumentException("Pris kan ikke være negativ");
        }
    }
} 
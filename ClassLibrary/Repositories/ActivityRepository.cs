using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Interfaces;
using ClassLibrary.Models;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Repository til håndtering af aktiviteter
    /// </summary>
    public class ActivityRepository : IActivityRepository
    {
        private readonly List<Activity> _activities = new();

        public async Task<IEnumerable<Activity>> GetAllAsync()
        {
            return await Task.FromResult(_activities.Where(a => !a.IsDeleted));
        }

        public async Task<Activity?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0", nameof(id));

            return await Task.FromResult(_activities.FirstOrDefault(a => a.Id == id && !a.IsDeleted));
        }

        public async Task<Activity> AddAsync(Activity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            ValidateActivity(entity);

            entity.Id = _activities.Count > 0 ? _activities.Max(a => a.Id) + 1 : 1;
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            _activities.Add(entity);
            return await Task.FromResult(entity);
        }

        public async Task<Activity> UpdateAsync(Activity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            ValidateActivity(entity);

            var existingActivity = _activities.FirstOrDefault(a => a.Id == entity.Id && !a.IsDeleted);
            if (existingActivity == null)
                throw new KeyNotFoundException($"Aktivitet med ID {entity.Id} blev ikke fundet");

            var index = _activities.IndexOf(existingActivity);
            entity.IsDeleted = existingActivity.IsDeleted;
            entity.DeletedAt = existingActivity.DeletedAt;
            _activities[index] = entity;
            return await Task.FromResult(entity);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0", nameof(id));

            var activity = _activities.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (activity == null)
                throw new KeyNotFoundException($"Aktivitet med ID {id} blev ikke fundet");

            activity.IsDeleted = true;
            activity.DeletedAt = DateTime.Now;
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Activity>> FindAsync(System.Linq.Expressions.Expression<Func<Activity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var compiledPredicate = predicate.Compile();
            return await Task.FromResult(_activities.Where(a => !a.IsDeleted && compiledPredicate(a)));
        }

        public async Task<(IEnumerable<Activity> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            System.Linq.Expressions.Expression<Func<Activity, bool>>? filter = null)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Sidenummer skal være større end 0", nameof(pageNumber));
            if (pageSize < 1)
                throw new ArgumentException("Sideantal skal være større end 0", nameof(pageSize));

            var query = _activities.Where(a => !a.IsDeleted).AsQueryable();
            
            if (filter != null)
            {
                var compiledFilter = filter.Compile();
                query = query.Where(compiledFilter).AsQueryable();
            }

            var totalCount = query.Count();
            var items = query.Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

            return await Task.FromResult((items, totalCount));
        }

        public async Task<IEnumerable<Activity>> FindAllAsync(IEnumerable<System.Linq.Expressions.Expression<Func<Activity, bool>>> predicates)
        {
            if (predicates == null)
                throw new ArgumentNullException(nameof(predicates));

            var query = _activities.Where(a => !a.IsDeleted).AsQueryable();
            foreach (var predicate in predicates)
            {
                if (predicate == null)
                    continue;

                var compiledPredicate = predicate.Compile();
                query = query.Where(compiledPredicate).AsQueryable();
            }
            return await Task.FromResult(query);
        }

        public async Task<IEnumerable<Activity>> FindAndSortAsync<TKey>(
            System.Linq.Expressions.Expression<Func<Activity, bool>> predicate,
            System.Linq.Expressions.Expression<Func<Activity, TKey>> sortKey,
            bool ascending = true)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (sortKey == null)
                throw new ArgumentNullException(nameof(sortKey));

            var compiledPredicate = predicate.Compile();
            var compiledSortKey = sortKey.Compile();
            
            var query = _activities.Where(a => !a.IsDeleted && compiledPredicate(a));
            query = ascending ? 
                query.OrderBy(compiledSortKey) : 
                query.OrderByDescending(compiledSortKey);

            return await Task.FromResult(query);
        }

        public async Task<IEnumerable<Activity>> FindAndTakeAsync(
            System.Linq.Expressions.Expression<Func<Activity, bool>> predicate,
            int count)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (count < 0)
                throw new ArgumentException("Antal skal være større end eller lig med 0", nameof(count));

            var compiledPredicate = predicate.Compile();
            return await Task.FromResult(_activities.Where(a => !a.IsDeleted && compiledPredicate(a)).Take(count));
        }

        public async Task<IEnumerable<Activity>> FindAndSkipAsync(
            System.Linq.Expressions.Expression<Func<Activity, bool>> predicate,
            int count)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (count < 0)
                throw new ArgumentException("Antal skal være større end eller lig med 0", nameof(count));

            var compiledPredicate = predicate.Compile();
            return await Task.FromResult(_activities.Where(a => !a.IsDeleted && compiledPredicate(a)).Skip(count));
        }

        public async Task<IDictionary<TKey, IEnumerable<Activity>>> FindAndGroupAsync<TKey>(
            System.Linq.Expressions.Expression<Func<Activity, bool>> predicate,
            System.Linq.Expressions.Expression<Func<Activity, TKey>> groupKey) where TKey : notnull
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (groupKey == null)
                throw new ArgumentNullException(nameof(groupKey));

            var compiledPredicate = predicate.Compile();
            var compiledGroupKey = groupKey.Compile();
            
            var grouped = _activities.Where(a => !a.IsDeleted && compiledPredicate(a))
                                   .GroupBy(compiledGroupKey)
                                   .ToDictionary(g => g.Key, g => g.AsEnumerable());

            return await Task.FromResult(grouped);
        }

        /// <summary>
        /// Finder alle kommende aktiviteter
        /// </summary>
        public async Task<IEnumerable<Activity>> GetUpcomingActivitiesAsync()
        {
            return await Task.FromResult(_activities.Where(a => !a.IsDeleted && a.ActivityDate > DateTime.Now));
        }

        /// <summary>
        /// Finder aktiviteter for en bestemt medarbejder
        /// </summary>
        public async Task<IEnumerable<Activity>> GetByEmployeeIdAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Medarbejder ID skal være større end 0", nameof(employeeId));

            return await Task.FromResult(_activities.Where(a => !a.IsDeleted && a.EmployeeId == employeeId));
        }

        /// <summary>
        /// Finder aktiviteter i et bestemt datointerval
        /// </summary>
        public async Task<IEnumerable<Activity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            return await Task.FromResult(_activities.Where(a => !a.IsDeleted && 
                a.ActivityDate >= startDate && 
                a.ActivityDate.AddMinutes(a.DurationMinutes) <= endDate));
        }

        /// <summary>
        /// Finder aktiviteter baseret på status
        /// </summary>
        public async Task<IEnumerable<Activity>> GetByStatusAsync(ActivityStatus status)
        {
            if (!Enum.IsDefined(typeof(ActivityStatus), status))
                throw new ArgumentException("Ugyldig aktivitetsstatus", nameof(status));

            return await Task.FromResult(_activities.Where(a => !a.IsDeleted && a.Status == status));
        }

        /// <summary>
        /// Finder aktiviteter der har ledige pladser
        /// </summary>
        public async Task<IEnumerable<Activity>> GetActivitiesWithAvailableSpotsAsync()
        {
            return await Task.FromResult(_activities.Where(a => !a.IsDeleted && !a.IsFullyBooked));
        }

        /// <summary>
        /// Finder aktiviteter baseret på lokation
        /// </summary>
        public async Task<IEnumerable<Activity>> GetByLocationAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Lokation kan ikke være tom", nameof(location));

            return await Task.FromResult(_activities.Where(a => !a.IsDeleted && 
                a.Location.Equals(location, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder aktiviteter baseret på prisinterval
        /// </summary>
        public async Task<IEnumerable<Activity>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0)
                throw new ArgumentException("Minimumspris kan ikke være negativ", nameof(minPrice));
            if (maxPrice < minPrice)
                throw new ArgumentException("Maksimumspris skal være større end minimumspris", nameof(maxPrice));

            return await Task.FromResult(_activities.Where(a => !a.IsDeleted && 
                a.Price >= minPrice && a.Price <= maxPrice));
        }

        /// <summary>
        /// Finder aktiviteter hvor en bestemt kunde er tilmeldt
        /// </summary>
        public async Task<IEnumerable<Activity>> GetActivitiesForParticipantAsync(int participantId)
        {
            if (participantId <= 0)
                throw new ArgumentException("Deltager ID skal være større end 0", nameof(participantId));

            return await Task.FromResult(_activities.Where(a => !a.IsDeleted && 
                a.ParticipantIds.Contains(participantId)));
        }

        /// <summary>
        /// Tilføjer en deltager til en aktivitet
        /// </summary>
        public async Task AddParticipantAsync(int activityId, int participantId)
        {
            if (activityId <= 0)
                throw new ArgumentException("Aktivitets ID skal være større end 0", nameof(activityId));
            if (participantId <= 0)
                throw new ArgumentException("Deltager ID skal være større end 0", nameof(participantId));

            var activity = await GetByIdAsync(activityId);
            if (activity == null)
                throw new KeyNotFoundException($"Aktivitet med ID {activityId} blev ikke fundet");

            activity.AddParticipant(participantId);
            await UpdateAsync(activity);
        }

        /// <summary>
        /// Fjerner en deltager fra en aktivitet
        /// </summary>
        public async Task RemoveParticipantAsync(int activityId, int participantId)
        {
            if (activityId <= 0)
                throw new ArgumentException("Aktivitets ID skal være større end 0", nameof(activityId));
            if (participantId <= 0)
                throw new ArgumentException("Deltager ID skal være større end 0", nameof(participantId));

            var activity = await GetByIdAsync(activityId);
            if (activity == null)
                throw new KeyNotFoundException($"Aktivitet med ID {activityId} blev ikke fundet");

            activity.RemoveParticipant(participantId);
            await UpdateAsync(activity);
        }

        /// <summary>
        /// Tjekker om en deltager er tilmeldt en aktivitet
        /// </summary>
        public async Task<bool> IsParticipantRegisteredAsync(int activityId, int participantId)
        {
            if (activityId <= 0)
                throw new ArgumentException("Aktivitets ID skal være større end 0", nameof(activityId));
            if (participantId <= 0)
                throw new ArgumentException("Deltager ID skal være større end 0", nameof(participantId));

            var activity = await GetByIdAsync(activityId);
            if (activity == null)
                throw new KeyNotFoundException($"Aktivitet med ID {activityId} blev ikke fundet");

            return activity.IsParticipantRegistered(participantId);
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

            if (!Enum.IsDefined(typeof(ActivityStatus), activity.Status))
                throw new ArgumentException("Ugyldig aktivitetsstatus");
        }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Interfaces;
using ClassLibrary.Models;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Repository til håndtering af adoptioner
    /// </summary>
    public class AdoptionRepository : IAdoptionRepository
    {
        private readonly List<Adoption> _adoptions = new();

        public async Task<IEnumerable<Adoption>> GetAllAsync()
        {
            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted));
        }

        public async Task<Adoption?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0", nameof(id));

            return await Task.FromResult(_adoptions.FirstOrDefault(a => a.Id == id && !a.IsDeleted));
        }

        public async Task<Adoption> AddAsync(Adoption entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            ValidateAdoption(entity);

            entity.Id = _adoptions.Count > 0 ? _adoptions.Max(a => a.Id) + 1 : 1;
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            _adoptions.Add(entity);
            return await Task.FromResult(entity);
        }

        public async Task<Adoption> UpdateAsync(Adoption entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            ValidateAdoption(entity);

            var existingAdoption = _adoptions.FirstOrDefault(a => a.Id == entity.Id && !a.IsDeleted);
            if (existingAdoption == null)
                throw new KeyNotFoundException($"Adoption med ID {entity.Id} blev ikke fundet");

            var index = _adoptions.IndexOf(existingAdoption);
            entity.IsDeleted = existingAdoption.IsDeleted;
            entity.DeletedAt = existingAdoption.DeletedAt;
            _adoptions[index] = entity;
            return await Task.FromResult(entity);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID skal være større end 0", nameof(id));

            var adoption = _adoptions.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (adoption == null)
                throw new KeyNotFoundException($"Adoption med ID {id} blev ikke fundet");

            adoption.IsDeleted = true;
            adoption.DeletedAt = DateTime.Now;
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Adoption>> FindAsync(System.Linq.Expressions.Expression<Func<Adoption, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var compiledPredicate = predicate.Compile();
            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && compiledPredicate(a)));
        }

        public async Task<(IEnumerable<Adoption> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            System.Linq.Expressions.Expression<Func<Adoption, bool>>? filter = null)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Sidenummer skal være større end 0", nameof(pageNumber));
            if (pageSize < 1)
                throw new ArgumentException("Sideantal skal være større end 0", nameof(pageSize));

            var query = _adoptions.Where(a => !a.IsDeleted).AsQueryable();
            
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

        public async Task<IEnumerable<Adoption>> FindAllAsync(IEnumerable<System.Linq.Expressions.Expression<Func<Adoption, bool>>> predicates)
        {
            if (predicates == null)
                throw new ArgumentNullException(nameof(predicates));

            var query = _adoptions.Where(a => !a.IsDeleted).AsQueryable();
            foreach (var predicate in predicates)
            {
                if (predicate == null)
                    continue;

                var compiledPredicate = predicate.Compile();
                query = query.Where(compiledPredicate).AsQueryable();
            }
            return await Task.FromResult(query);
        }

        public async Task<IEnumerable<Adoption>> FindAndSortAsync<TKey>(
            System.Linq.Expressions.Expression<Func<Adoption, bool>> predicate,
            System.Linq.Expressions.Expression<Func<Adoption, TKey>> sortKey,
            bool ascending = true)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (sortKey == null)
                throw new ArgumentNullException(nameof(sortKey));

            var compiledPredicate = predicate.Compile();
            var compiledSortKey = sortKey.Compile();
            
            var query = _adoptions.Where(a => !a.IsDeleted && compiledPredicate(a));
            query = ascending ? 
                query.OrderBy(compiledSortKey) : 
                query.OrderByDescending(compiledSortKey);

            return await Task.FromResult(query);
        }

        public async Task<IEnumerable<Adoption>> FindAndTakeAsync(
            System.Linq.Expressions.Expression<Func<Adoption, bool>> predicate,
            int count)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (count < 0)
                throw new ArgumentException("Antal skal være større end eller lig med 0", nameof(count));

            var compiledPredicate = predicate.Compile();
            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && compiledPredicate(a)).Take(count));
        }

        public async Task<IEnumerable<Adoption>> FindAndSkipAsync(
            System.Linq.Expressions.Expression<Func<Adoption, bool>> predicate,
            int count)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (count < 0)
                throw new ArgumentException("Antal skal være større end eller lig med 0", nameof(count));

            var compiledPredicate = predicate.Compile();
            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && compiledPredicate(a)).Skip(count));
        }

        public async Task<IDictionary<TKey, IEnumerable<Adoption>>> FindAndGroupAsync<TKey>(
            System.Linq.Expressions.Expression<Func<Adoption, bool>> predicate,
            System.Linq.Expressions.Expression<Func<Adoption, TKey>> groupKey) where TKey : notnull
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (groupKey == null)
                throw new ArgumentNullException(nameof(groupKey));

            var compiledPredicate = predicate.Compile();
            var compiledGroupKey = groupKey.Compile();
            
            var grouped = _adoptions.Where(a => !a.IsDeleted && compiledPredicate(a))
                                   .GroupBy(compiledGroupKey)
                                   .ToDictionary(g => g.Key, g => g.AsEnumerable());

            return await Task.FromResult(grouped);
        }

        public async Task<IEnumerable<Adoption>> GetByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("Kunde ID skal være større end 0", nameof(customerId));

            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && a.CustomerId == customerId));
        }

        public async Task<IEnumerable<Adoption>> GetByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("Dyr ID skal være større end 0", nameof(animalId));

            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && a.AnimalId == animalId));
        }

        public async Task<IEnumerable<Adoption>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && 
                a.AdoptionDate >= startDate && a.AdoptionDate <= endDate));
        }

        public async Task<IEnumerable<Adoption>> GetByStatusAsync(AdoptionStatus status)
        {
            if (!Enum.IsDefined(typeof(AdoptionStatus), status))
                throw new ArgumentException("Ugyldig adoptionsstatus", nameof(status));

            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && a.Status == status));
        }

        public async Task<IEnumerable<Adoption>> GetBySpeciesAsync(Species species)
        {
            if (!Enum.IsDefined(typeof(Species), species))
                throw new ArgumentException("Ugyldig dyreart", nameof(species));

            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && 
                a.Animal != null && a.Animal.Species == species));
        }

        public async Task<IEnumerable<Adoption>> GetByAgeInYearsAsync(int years)
        {
            if (years < 0)
                throw new ArgumentException("Alder kan ikke være negativ", nameof(years));

            var targetDate = DateTime.Now.AddYears(-years);
            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && 
                a.Animal != null && a.Animal.BirthDate <= targetDate && 
                a.Animal.BirthDate > targetDate.AddYears(-1)));
        }

        public async Task<IEnumerable<Adoption>> GetByAgeInMonthsAsync(int months)
        {
            if (months < 0)
                throw new ArgumentException("Alder kan ikke være negativ", nameof(months));

            var targetDate = DateTime.Now.AddMonths(-months);
            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && 
                a.Animal != null && a.Animal.BirthDate <= targetDate && 
                a.Animal.BirthDate > targetDate.AddMonths(-1)));
        }

        public async Task<IEnumerable<Adoption>> GetByAgeInWeeksAsync(int weeks)
        {
            if (weeks < 0)
                throw new ArgumentException("Alder kan ikke være negativ", nameof(weeks));

            var targetDate = DateTime.Now.AddDays(-weeks * 7);
            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && 
                a.Animal != null && a.Animal.BirthDate <= targetDate && 
                a.Animal.BirthDate > targetDate.AddDays(-7)));
        }

        public async Task<IEnumerable<Adoption>> GetByAgeRangeInYearsAsync(int minYears, int maxYears)
        {
            if (minYears < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ", nameof(minYears));
            if (maxYears < minYears)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder", nameof(maxYears));

            var maxDate = DateTime.Now.AddYears(-minYears);
            var minDate = DateTime.Now.AddYears(-maxYears - 1);
            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && 
                a.Animal != null && a.Animal.BirthDate <= maxDate && 
                a.Animal.BirthDate > minDate));
        }

        public async Task<IEnumerable<Adoption>> GetByAgeRangeInMonthsAsync(int minMonths, int maxMonths)
        {
            if (minMonths < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ", nameof(minMonths));
            if (maxMonths < minMonths)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder", nameof(maxMonths));

            var maxDate = DateTime.Now.AddMonths(-minMonths);
            var minDate = DateTime.Now.AddMonths(-maxMonths - 1);
            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && 
                a.Animal != null && a.Animal.BirthDate <= maxDate && 
                a.Animal.BirthDate > minDate));
        }

        public async Task<IEnumerable<Adoption>> GetByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks)
        {
            if (minWeeks < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ", nameof(minWeeks));
            if (maxWeeks < minWeeks)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder", nameof(maxWeeks));

            var maxDate = DateTime.Now.AddDays(-minWeeks * 7);
            var minDate = DateTime.Now.AddDays(-maxWeeks * 7 - 7);
            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && 
                a.Animal != null && a.Animal.BirthDate <= maxDate && 
                a.Animal.BirthDate > minDate));
        }

        public async Task<IEnumerable<Adoption>> GetByAdoptionTypeAsync(string adoptionType)
        {
            if (string.IsNullOrWhiteSpace(adoptionType))
                throw new ArgumentException("Adoptionstype kan ikke være tom", nameof(adoptionType));

            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && 
                a.AdoptionType.Equals(adoptionType, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<Adoption>> GetByEmployeeIdAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Medarbejder ID skal være større end 0", nameof(employeeId));

            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && a.EmployeeId == employeeId));
        }

        public async Task<IEnumerable<Adoption>> GetByAdopterEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email kan ikke være tom", nameof(email));

            return await Task.FromResult(_adoptions.Where(a => !a.IsDeleted && 
                a.AdopterEmail.Equals(email, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<Adoption> GetLatestAdoptionForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("Dyr ID skal være større end 0", nameof(animalId));

            var adoption = _adoptions.Where(a => !a.IsDeleted && a.AnimalId == animalId)
                                   .OrderByDescending(a => a.AdoptionDate)
                                   .FirstOrDefault();

            if (adoption == null)
                throw new KeyNotFoundException($"Ingen adoption fundet for dyr med ID {animalId}");

            return await Task.FromResult(adoption);
        }

        /// <summary>
        /// Validerer en adoption
        /// </summary>
        private void ValidateAdoption(Adoption adoption)
        {
            if (adoption.CustomerId <= 0)
                throw new ArgumentException("Kunde ID skal være større end 0");

            if (adoption.AnimalId <= 0)
                throw new ArgumentException("Dyr ID skal være større end 0");

            if (adoption.EmployeeId <= 0)
                throw new ArgumentException("Medarbejder ID skal være større end 0");

            if (string.IsNullOrWhiteSpace(adoption.AdoptionType))
                throw new ArgumentException("Adoptionstype kan ikke være tom");

            if (adoption.AdoptionDate > DateTime.Now)
                throw new ArgumentException("Adoptionsdato kan ikke være i fremtiden");

            if (!Enum.IsDefined(typeof(AdoptionStatus), adoption.Status))
                throw new ArgumentException("Ugyldig adoptionsstatus");
        }
    }
} 
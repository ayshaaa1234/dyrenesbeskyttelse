using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Repository til håndtering af adoptioner
    /// </summary>
    public class AdoptionRepository : Repository<Adoption>, IAdoptionRepository
    {
        public AdoptionRepository() : base()
        {
        }

        /// <summary>
        /// Finder alle adoptioner for en bestemt kunde
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("CustomerId skal være større end 0");

            return Task.FromResult(_items.Where(a => a.CustomerId == customerId));
        }

        /// <summary>
        /// Finder alle adoptioner for et bestemt dyr
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByAnimalIdAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            return Task.FromResult(_items.Where(a => a.AnimalId == animalId));
        }

        /// <summary>
        /// Finder adoptioner i et bestemt datointerval
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            return Task.FromResult(_items.Where(a => 
                a.AdoptionDate >= startDate && a.AdoptionDate <= endDate));
        }

        /// <summary>
        /// Finder adoptioner baseret på status
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByStatusAsync(AdoptionStatus status)
        {
            return Task.FromResult(_items.Where(a => a.Status == status));
        }

        /// <summary>
        /// Finder adoptioner baseret på dyreart
        /// </summary>
        public Task<IEnumerable<Adoption>> GetBySpeciesAsync(Species species)
        {
            return Task.FromResult(_items.Where(a => a.Animal != null && a.Animal.Species == species));
        }

        /// <summary>
        /// Finder adoptioner baseret på alder i år
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByAgeInYearsAsync(int years)
        {
            if (years < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return Task.FromResult(_items.Where(a => a.Animal != null && a.Animal.GetAgeInYears() == years));
        }

        /// <summary>
        /// Finder adoptioner baseret på alder i måneder
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByAgeInMonthsAsync(int months)
        {
            if (months < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return Task.FromResult(_items.Where(a => a.Animal != null && a.Animal.GetAgeInMonths() == months));
        }

        /// <summary>
        /// Finder adoptioner baseret på alder i uger
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByAgeInWeeksAsync(int weeks)
        {
            if (weeks < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return Task.FromResult(_items.Where(a => a.Animal != null && a.Animal.GetAgeInWeeks() == weeks));
        }

        /// <summary>
        /// Finder adoptioner baseret på aldersinterval i år
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByAgeRangeInYearsAsync(int minYears, int maxYears)
        {
            if (minYears < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxYears < minYears)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return Task.FromResult(_items.Where(a => 
                a.Animal != null && 
                a.Animal.GetAgeInYears() >= minYears && 
                a.Animal.GetAgeInYears() <= maxYears));
        }

        /// <summary>
        /// Finder adoptioner baseret på aldersinterval i måneder
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByAgeRangeInMonthsAsync(int minMonths, int maxMonths)
        {
            if (minMonths < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxMonths < minMonths)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return Task.FromResult(_items.Where(a => 
                a.Animal != null && 
                a.Animal.GetAgeInMonths() >= minMonths && 
                a.Animal.GetAgeInMonths() <= maxMonths));
        }

        /// <summary>
        /// Finder adoptioner baseret på aldersinterval i uger
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks)
        {
            if (minWeeks < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxWeeks < minWeeks)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return Task.FromResult(_items.Where(a => 
                a.Animal != null && 
                a.Animal.GetAgeInWeeks() >= minWeeks && 
                a.Animal.GetAgeInWeeks() <= maxWeeks));
        }

        /// <summary>
        /// Finder adoptioner baseret på adoptionstype
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByAdoptionTypeAsync(string adoptionType)
        {
            if (string.IsNullOrWhiteSpace(adoptionType))
                throw new ArgumentException("Adoptionstype kan ikke være tom");

            return Task.FromResult(_items.Where(a => 
                a.AdoptionType.Equals(adoptionType, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder adoptioner baseret på medarbejder
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByEmployeeIdAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("EmployeeId skal være større end 0");

            return Task.FromResult(_items.Where(a => a.EmployeeId == employeeId));
        }

        /// <summary>
        /// Finder alle adoptioner for en bestemt adopter
        /// </summary>
        public Task<IEnumerable<Adoption>> GetByAdopterEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email kan ikke være tom");

            return Task.FromResult(_items.Where(a => 
                a.AdopterEmail.Equals(email, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder den seneste adoption for et dyr
        /// </summary>
        public Task<Adoption> GetLatestAdoptionForAnimalAsync(int animalId)
        {
            if (animalId <= 0)
                throw new ArgumentException("AnimalId skal være større end 0");

            var adoption = _items
                .Where(a => a.AnimalId == animalId)
                .OrderByDescending(a => a.AdoptionDate)
                .FirstOrDefault();

            if (adoption == null)
                throw new KeyNotFoundException($"Ingen adoption fundet for dyr med ID: {animalId}");

            return Task.FromResult(adoption);
        }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Repository til håndtering af dyr
    /// </summary>
    public class AnimalRepository : Repository<Animal>, IAnimalRepository
    {
        public AnimalRepository() : base()
        {
        }

        /// <summary>
        /// Finder dyr baseret på dyreart
        /// </summary>
        public Task<IEnumerable<Animal>> GetBySpeciesAsync(Species species)
        {
            return Task.FromResult(_items.Where(a => a.Species == species));
        }

        /// <summary>
        /// Finder dyr baseret på alder i år
        /// </summary>
        public Task<IEnumerable<Animal>> GetByAgeInYearsAsync(int years)
        {
            if (years < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return Task.FromResult(_items.Where(a => 
                a.BirthDate.HasValue && a.GetAgeInYears() == years));
        }

        /// <summary>
        /// Finder dyr baseret på alder i måneder
        /// </summary>
        public Task<IEnumerable<Animal>> GetByAgeInMonthsAsync(int months)
        {
            if (months < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return Task.FromResult(_items.Where(a => 
                a.BirthDate.HasValue && a.GetAgeInMonths() == months));
        }

        /// <summary>
        /// Finder dyr baseret på alder i uger
        /// </summary>
        public Task<IEnumerable<Animal>> GetByAgeInWeeksAsync(int weeks)
        {
            if (weeks < 0)
                throw new ArgumentException("Alder kan ikke være negativ");

            return Task.FromResult(_items.Where(a => 
                a.BirthDate.HasValue && a.GetAgeInWeeks() == weeks));
        }

        /// <summary>
        /// Finder dyr baseret på køn
        /// </summary>
        public Task<IEnumerable<Animal>> GetByGenderAsync(string gender)
        {
            if (string.IsNullOrWhiteSpace(gender))
                throw new ArgumentException("Køn kan ikke være tomt");

            return Task.FromResult(_items.Where(a => 
                a.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder dyr baseret på vægtinterval
        /// </summary>
        public Task<IEnumerable<Animal>> GetByWeightRangeAsync(decimal minWeight, decimal maxWeight)
        {
            if (minWeight < 0)
                throw new ArgumentException("Minimumsvægt kan ikke være negativ");
            if (maxWeight < minWeight)
                throw new ArgumentException("Maksimumsvægt skal være større end minimumsvægt");

            return Task.FromResult(_items.Where(a => 
                a.Weight >= minWeight && a.Weight <= maxWeight));
        }

        /// <summary>
        /// Finder dyr baseret på aldersinterval i år
        /// </summary>
        public Task<IEnumerable<Animal>> GetByAgeRangeInYearsAsync(int minYears, int maxYears)
        {
            if (minYears < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxYears < minYears)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return Task.FromResult(_items.Where(a => 
                a.BirthDate.HasValue && 
                a.GetAgeInYears() >= minYears && 
                a.GetAgeInYears() <= maxYears));
        }

        /// <summary>
        /// Finder dyr baseret på aldersinterval i måneder
        /// </summary>
        public Task<IEnumerable<Animal>> GetByAgeRangeInMonthsAsync(int minMonths, int maxMonths)
        {
            if (minMonths < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxMonths < minMonths)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return Task.FromResult(_items.Where(a => 
                a.BirthDate.HasValue && 
                a.GetAgeInMonths() >= minMonths && 
                a.GetAgeInMonths() <= maxMonths));
        }

        /// <summary>
        /// Finder dyr baseret på aldersinterval i uger
        /// </summary>
        public Task<IEnumerable<Animal>> GetByAgeRangeInWeeksAsync(int minWeeks, int maxWeeks)
        {
            if (minWeeks < 0)
                throw new ArgumentException("Minimumsalder kan ikke være negativ");
            if (maxWeeks < minWeeks)
                throw new ArgumentException("Maksimumsalder skal være større end minimumsalder");

            return Task.FromResult(_items.Where(a => 
                a.BirthDate.HasValue && 
                a.GetAgeInWeeks() >= minWeeks && 
                a.GetAgeInWeeks() <= maxWeeks));
        }

        /// <summary>
        /// Finder dyr baseret på adoptionsstatus
        /// </summary>
        public Task<IEnumerable<Animal>> GetByAdoptionStatusAsync(bool isAdopted)
        {
            return Task.FromResult(_items.Where(a => a.IsAdopted == isAdopted));
        }

        /// <summary>
        /// Finder dyr baseret på indkomstdato
        /// </summary>
        public Task<IEnumerable<Animal>> GetByIntakeDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Startdato skal være før slutdato");

            return Task.FromResult(_items.Where(a => 
                a.IntakeDate >= startDate && a.IntakeDate <= endDate));
        }

        /// <summary>
        /// Finder dyr baseret på navn
        /// </summary>
        public Task<IEnumerable<Animal>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Navn kan ikke være tomt");

            return Task.FromResult(_items.Where(a => 
                a.Name.Contains(name, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder dyr baseret på race
        /// </summary>
        public Task<IEnumerable<Animal>> GetByBreedAsync(string breed)
        {
            if (string.IsNullOrWhiteSpace(breed))
                throw new ArgumentException("Race kan ikke være tom");

            return Task.FromResult(_items.Where(a => 
                a.Breed.Equals(breed, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Finder alle dyr der er adopteret
        /// </summary>
        public Task<IEnumerable<Animal>> GetAdoptedAnimalsAsync()
        {
            return Task.FromResult(_items.Where(a => a.IsAdopted));
        }

        /// <summary>
        /// Finder alle dyr der ikke er adopteret
        /// </summary>
        public Task<IEnumerable<Animal>> GetAvailableAnimalsAsync()
        {
            return Task.FromResult(_items.Where(a => !a.IsAdopted));
        }

        /// <summary>
        /// Finder dyr der skal til lægen i en bestemt uge
        /// </summary>
        public Task<IEnumerable<Animal>> GetAnimalsWithVetAppointmentsInWeekAsync(int weekNumber)
        {
            if (weekNumber < 1 || weekNumber > 53)
                throw new ArgumentException("Ugenummer skal være mellem 1 og 53");

            return Task.FromResult(_items.Where(a => 
                a.HealthRecords.Any(h => 
                    h.AppointmentDate.HasValue && 
                    GetWeekNumber(h.AppointmentDate.Value) == weekNumber)));
        }

        private int GetWeekNumber(DateTime date)
        {
            return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                date,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.Features.AnimalManagement.Core.Models;
using ClassLibrary.Features.AnimalManagement.Core.Enums;
using ClassLibrary.Features.AnimalManagement.Infrastructure.Abstractions;
using ClassLibrary.SharedKernel.Persistence.Implementations;
// using ClassLibrary.SharedKernel.Domain.Abstractions; // Ikke længere nødvendig direkte her

namespace ClassLibrary.Features.AnimalManagement.Infrastructure.Implementations
{
    public class AnimalRepository : Repository<Animal>, IAnimalRepository
    {
        private readonly IHealthRecordRepository _healthRecordRepository;
        // Definer filstien. Dette kan senere centraliseres.
        private const string FilePath = "Data/Json/animals.json";

        public AnimalRepository(IHealthRecordRepository healthRecordRepository) : base(FilePath)
        {
            _healthRecordRepository = healthRecordRepository ?? throw new ArgumentNullException(nameof(healthRecordRepository));
        }

        // AddAsync i base Repository<T> håndterer nu ID-generering, 
        // hvis ID'et på entiteten er 0. 
        // Den tidligere override af AddAsync her er derfor ikke nødvendig for ID-generering.
        // Vi beholder dog ValidateEntity.

        public override async Task<Animal> AddAsync(Animal entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity); // Kald altid validering
            return await base.AddAsync(entity); // Kald base AddAsync som nu håndterer ID korrekt
        }

        public override async Task<Animal> UpdateAsync(Animal entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            ValidateEntity(entity); // Kald altid validering ved opdatering også
            return await base.UpdateAsync(entity);
        }


        protected override void ValidateEntity(Animal entity)
        {
            base.ValidateEntity(entity); // Kalder base null check
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Dyrenavn kan ikke være tomt.", nameof(entity.Name));
            if (!Enum.IsDefined(typeof(Species), entity.Species))
                throw new ArgumentException("Ugyldig dyreart.", nameof(entity.Species));
            // Yderligere valideringer kan tilføjes her
        }

        public async Task<IEnumerable<Animal>> GetAvailableAnimalsAsync()
        {
            // base.FindAsync håndterer allerede LoadDataAsync()
            return await base.FindAsync(a => a.Status == AnimalStatus.Available);
        }

        public async Task<IEnumerable<Animal>> GetAdoptedAnimalsAsync()
        {
            return await base.FindAsync(a => a.Status == AnimalStatus.Adopted);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsBySpeciesAsync(Species species)
        {
            return await base.FindAsync(a => a.Species == species);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeInYearsAsync(int age)
        {
            var today = DateTime.Today;
            return await base.FindAsync(a => a.BirthDate.HasValue && (today.Year - a.BirthDate.Value.Year - (a.BirthDate.Value.DayOfYear > today.DayOfYear ? 1 : 0)) == age);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeInMonthsAsync(int ageInMonths)
        { 
            var today = DateTime.Today;
            return await base.FindAsync(a => a.BirthDate.HasValue && (((today.Year - a.BirthDate.Value.Year) * 12) + today.Month - a.BirthDate.Value.Month - (a.BirthDate.Value.Day > today.Day ? 1: 0)) == ageInMonths);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeInWeeksAsync(int ageInWeeks)
        {
            var today = DateTime.Today;
            return await base.FindAsync(a => a.BirthDate.HasValue && (int)((today - a.BirthDate.Value).TotalDays / 7) == ageInWeeks);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByGenderAsync(string gender)
        {
            return await base.FindAsync(a => !string.IsNullOrWhiteSpace(a.Gender) && string.Equals(a.Gender, gender, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByWeightRangeAsync(decimal minWeight, decimal maxWeight)
        {
            if (minWeight > maxWeight) throw new ArgumentException("Minimum vægt kan ikke være større end maximum vægt.");
            return await base.FindAsync(a => a.Weight >= minWeight && a.Weight <= maxWeight);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInYearsAsync(int minAge, int maxAge)
        {
            if (minAge > maxAge) throw new ArgumentException("Minimum alder kan ikke være større end maximum alder.");
            var today = DateTime.Today;
            return await base.FindAsync(a => 
                a.BirthDate.HasValue && 
                (today.Year - a.BirthDate.Value.Year - (a.BirthDate.Value.DayOfYear > today.DayOfYear ? 1 : 0)) >= minAge &&
                (today.Year - a.BirthDate.Value.Year - (a.BirthDate.Value.DayOfYear > today.DayOfYear ? 1 : 0)) <= maxAge
            );
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInMonthsAsync(int minAgeInMonths, int maxAgeInMonths)
        {
            if (minAgeInMonths > maxAgeInMonths) throw new ArgumentException("Minimum alder i måneder kan ikke være større end maximum alder i måneder.");
            var today = DateTime.Today;
            return await base.FindAsync(a => 
                a.BirthDate.HasValue && 
                (((today.Year - a.BirthDate.Value.Year) * 12) + today.Month - a.BirthDate.Value.Month - (a.BirthDate.Value.Day > today.Day ? 1: 0)) >= minAgeInMonths &&
                (((today.Year - a.BirthDate.Value.Year) * 12) + today.Month - a.BirthDate.Value.Month - (a.BirthDate.Value.Day > today.Day ? 1: 0)) <= maxAgeInMonths
            );
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAgeRangeInWeeksAsync(int minAgeInWeeks, int maxAgeInWeeks)
        {
            if (minAgeInWeeks > maxAgeInWeeks) throw new ArgumentException("Minimum alder i uger kan ikke være større end maximum alder i uger.");
            var today = DateTime.Today;
            return await base.FindAsync(a => 
                a.BirthDate.HasValue && 
                (int)((today - a.BirthDate.Value).TotalDays / 7) >= minAgeInWeeks &&
                (int)((today - a.BirthDate.Value).TotalDays / 7) <= maxAgeInWeeks
            );
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByAdoptionStatusAsync(AnimalStatus status)
        {
            return await base.FindAsync(a => a.Status == status);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByIntakeDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) throw new ArgumentException("Startdato kan ikke være efter slutdato.");
            return await base.FindAsync(a => a.IntakeDate.Date >= startDate.Date && a.IntakeDate.Date <= endDate.Date);
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return await GetAllAsync(); 
            return await base.FindAsync(a => !string.IsNullOrWhiteSpace(a.Name) && a.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Animal>> GetAnimalsByBreedAsync(string breed)
        {
            if (string.IsNullOrWhiteSpace(breed)) return await GetAllAsync();
            return await base.FindAsync(a => !string.IsNullOrWhiteSpace(a.Breed) && a.Breed.Contains(breed, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Animal>> GetAnimalsNeedingVaccinationAsync()
        {
            var animalsInCare = await base.FindAsync(a => 
                a.Status == AnimalStatus.Available || 
                a.Status == AnimalStatus.Reserved || 
                a.Status == AnimalStatus.InTreatment
            );

            var animalsNeedingVaccination = new List<Animal>();
            var today = DateTime.Today;

            foreach (var animal in animalsInCare)
            {
                var healthRecords = await _healthRecordRepository.GetHealthRecordsByAnimalIdAsync(animal.Id);
                var latestVaccinationRecord = healthRecords
                    .Where(hr => hr.IsVaccinated) // Antager HealthRecord har et IsVaccinated felt
                    .OrderByDescending(hr => hr.RecordDate) // Eller VaccinationDate, hvis det findes
                    .FirstOrDefault();

                if (latestVaccinationRecord == null)
                {
                    // Ingen tidligere vaccination registreret
                    animalsNeedingVaccination.Add(animal);
                }
                else if (latestVaccinationRecord.NextVaccinationDate.HasValue && 
                         latestVaccinationRecord.NextVaccinationDate.Value.Date <= today)
                {
                    // Næste vaccination er i dag eller tidligere
                    animalsNeedingVaccination.Add(animal);
                }
                // Overvej også dyr uden NextVaccinationDate, men hvor sidste vaccination er "gammel"
                // Dette kræver en forretningsregel for, hvor længe en standardvaccination holder.
                // For nu holder vi os til NextVaccinationDate.
            }
            return animalsNeedingVaccination;
        }
    }
} 